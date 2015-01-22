using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Extensions;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Host
{
    public class SubscriptionFactory : ISubscriptionFactory
    {
        private readonly IErrorsInbox  _errorsInbox;
        private readonly IDomainPublisher _domainPublisher;
        private readonly IDomainPersistor _domainPersistor;
        private readonly INotifier _notifier;
        private readonly Func<ISubscription>[] _subscriptors;

        private readonly AtomicImmutableDictionary<string, LayeredDisposable> _subscriptions = new AtomicImmutableDictionary<string, LayeredDisposable>(); 

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

            var groupAdditions =
                from p in userAdditions
                from app in p.Target.Primary.Applications
                from token in p.Target.Secondary.Tokens
                select new { token, app = app.Name };
            groupAdditions.Subscribe(p => _notifier.AddGroup(p.token, p.app));


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

            var groupRemovals =
                from p in userRemovals
                from app in p.Target.Primary.Applications
                from token in p.Target.Secondary.Tokens
                select new { token, app = app.Name };
            groupRemovals.Subscribe(p => _notifier.RemoveGroup(p.token, p.app));


            //apps deltas

            var appDeltas =
                from p in _domainPublisher.GetClusterApplicationsSequence()
                let action = p.Type == DeltaType.Added
                             ? new Action<string, string>((t, g) => _notifier.AddGroup(t, g))
                             : (t, g) => _notifier.RemoveGroup(t, g)
                let target = p.Target.Secondary.Name
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
                    .Each(t => p.Action(t, p.Target)));

            appDeltas.Subscribe(p =>
                _notifier.UserApplications(p.User.Name, p.Additions, p.Removals));
        }

        public async void Subscribe(string user, string token, Action<string> connector)
        {
            Func<IEnumerable<Application>> getUserApps = () => _domainPersistor.GetUserApplications(user).Result;

            var ut = await _domainPersistor.AddUserToken(user, token);

            //errors
            if (ut.HasValue && ut.Value.Tokens.Count() == 1)
            {
                var subscriptions =
                    from subscriptor in _subscriptors
                    select subscriptor().Subscribe(user, _notifier, _errorsInbox, _domainPersistor, _domainPublisher);

                var d = new CompositeDisposable(subscriptions.ToArray()).ToLayeredDisposable();

                _subscriptions.SetItem(user, d);
            }
            else
                _subscriptions.Get(user).Wrap();

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

    public interface ISubscription
    {
        IDisposable Subscribe(string user, INotifier notifier, IErrorsInbox errorsInbox, IDomainPersistor domainPersistor, IDomainPublisher domainPublisher);
    }

    public class ErrorsSubscription : ISubscription
    {
        public IDisposable Subscribe(string user, INotifier notifier, IErrorsInbox errorsInbox, IDomainPersistor domainPersistor, IDomainPublisher domainPublisher)
        {
            Func<IEnumerable<Application>> getUserApps = () => domainPersistor.GetUserApplications(user).Result;

            var errors =
                from e in errorsInbox.GetErrorsStream()
                from a in getUserApps().ToObservable()
                where e.SourceId == a.Name
                select e;

            return errors
                .Subscribe(payload => notifier.Error(user, payload));
        }
    }

    public class RecapsSubscription : ISubscription
    {
        public IDisposable Subscribe(string user, INotifier notifier, IErrorsInbox errorsInbox, IDomainPersistor domainPersistor, IDomainPublisher domainPublisher)
        {
            var userApps =
                from p in domainPublisher.GetClusterApplicationsSequence()
                let target = p.Target.Secondary.Name
                from u in p.Target.Primary.Users
                where u.Name == user
                select p.Target.Primary.Applications;

            //SOTW
            Func<Task<ValueOrError<Recap>>> initialRecap = async () => await errorsInbox.GetApplicationsRecap(await domainPersistor.GetUserApplications(user));
            var result = initialRecap().Result;
            if (result.HasValue)
                notifier.Recap(user, result.Value);

            return userApps
                .Subscribe(async payload =>
                {
                    var recap = await errorsInbox.GetApplicationsRecap(payload);
                    if (recap.HasValue)
                        notifier.Recap(user, result.Value);
                });
        }
    }
}