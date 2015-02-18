using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using RElmah.Extensions;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.StandingQueries
{
    public class StandingQueriesFactory : IStandingQueriesFactory
    {
        private readonly IErrorsInbox  _errorsInbox;
        private readonly IDomainPublisher _domainPublisher;
        private readonly IDomainPersistor _domainPersistor;
        private readonly INotifier _notifier;
        private readonly Func<IStandingQuery>[] _subscriptors;

        private readonly AtomicImmutableDictionary<string, LayeredDisposable> _subscriptions = new AtomicImmutableDictionary<string, LayeredDisposable>();

        public StandingQueriesFactory(IErrorsInbox errorsInbox, IDomainPublisher domainPublisher, IDomainPersistor domainPersistor,  
            INotifier notifier,
            params Func<IStandingQuery>[] subscriptors)
        {
            _errorsInbox  = errorsInbox;
            _domainPublisher = domainPublisher;
            _domainPersistor = domainPersistor;
            _notifier = notifier;
            _subscriptors = subscriptors;
        }

        public async void Setup(string user, string token, Action<string> connector)
        {
            Func<IEnumerable<Application>> getUserApps = () => _domainPersistor.GetUserApplications(user).Result;

            var ut = await _domainPersistor.AddUserToken(user, token);

            var subscriptions =
                from subscriptor in _subscriptors
                select subscriptor().Run(ut, _notifier, _errorsInbox, _domainPersistor, _domainPublisher);

            var d = new CompositeDisposable(subscriptions.ToArray()).ToLayeredDisposable();

            _subscriptions.SetItem(user, d);

            //apps
            getUserApps().Do(app => connector(app.Name));
        }

        public async void Teardown(string token)
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