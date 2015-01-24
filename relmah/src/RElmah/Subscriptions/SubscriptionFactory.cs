using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using RElmah.Common;
using RElmah.Extensions;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Subscriptions
{
    public class SubscriptionFactory : ISubscriptionFactory
    {
        private readonly IErrorsInbox  _errorsInbox;
        private readonly IDomainPublisher _domainPublisher;
        private readonly IDomainPersistor _domainPersistor;
        private readonly INotifier _notifier;
        private readonly Func<ISubscription>[] _subscriptors;

        private readonly AtomicImmutableDictionary<string, LayeredDisposable> _subscriptions = new AtomicImmutableDictionary<string, LayeredDisposable>();
        private static readonly Action<INotifier, string, string> Adder   = (n, t, g) => n.AddGroup(t, g);
        private static readonly Action<INotifier, string, string> Remover = (n, t, g) => n.RemoveGroup(t, g);

        public SubscriptionFactory(IErrorsInbox errorsInbox, IDomainPublisher domainPublisher, IDomainPersistor domainPersistor,  
            INotifier notifier,
            params Func<ISubscription>[] subscriptors)
        {
            _errorsInbox  = errorsInbox;
            _domainPublisher = domainPublisher;
            _domainPersistor = domainPersistor;
            _notifier = notifier;
            _subscriptors = subscriptors;
        }

        public void Start()
        {
            //user additions
            var userAdditions =
                from p in _domainPublisher.GetClusterUsersSequence()
                where p.Type == DeltaType.Added
                select p;
            userAdditions.Subscribe(
                u => _notifier.UserApplications(
                    u.Target.Secondary.Name, 
                    from a in u.Target.Primary.Applications
                    select a.Name,
                    Enumerable.Empty<string>()));

            //User removals
            var userRemovals =
                from p in _domainPublisher.GetClusterUsersSequence()
                where p.Type == DeltaType.Removed
                select p;
            userRemovals.Subscribe(
                u => _notifier.UserApplications(
                    u.Target.Secondary.Name, 
                    Enumerable.Empty<string>(),
                    from a in u.Target.Primary.Applications
                    select a.Name));

            //apps deltas
            var appDeltas =
                from p in _domainPublisher.GetClusterApplicationsSequence()
                let action   = p.Type == DeltaType.Added
                             ? Adder
                             : Remover
                let target   = p.Target.Secondary.Name
                let removals = p.Type == DeltaType.Added
                             ? Enumerable.Empty<string>()
                             : new[] { target }
                from user in p.Target.Primary.Users
                select new
                {
                    p.Type,
                    User = user,
                    Action = action,
                    Additions = from a in p.Target.Primary.Applications
                                select a.Name,
                    Removals = removals,
                    Target = target
                };

            appDeltas.Subscribe(p =>
                p.User.Tokens
                    .Each(t => p.Action(_notifier, t, p.Target)));

            appDeltas.Subscribe(p =>
                _notifier.UserApplications(p.User.Name, p.Additions, p.Removals));
        }

        public async void Subscribe(string user, string token, Action<string> connector)
        {
            Func<IEnumerable<Application>> getUserApps = () => _domainPersistor.GetUserApplications(user).Result;

            var ut = await _domainPersistor.AddUserToken(user, token);

            var subscriptions =
                from subscriptor in _subscriptors
                select subscriptor().Subscribe(ut, _notifier, _errorsInbox, _domainPersistor, _domainPublisher);

            var d = new CompositeDisposable(subscriptions.ToArray()).ToLayeredDisposable();

            _subscriptions.SetItem(user, d);

            //apps
            getUserApps().Do(app => connector(app.Name));
        }

        public async void Disconnect(string token)
        {
            var u = await _domainPersistor.RemoveUserToken(token);
            if (!u.HasValue) return;

            var name         = u.Value.Name;
            var subscription = _subscriptions.Get(name);

            subscription.Dispose();

            if (subscription.IsDisposed)
                _subscriptions.Remove(name);
        }       
    }
}