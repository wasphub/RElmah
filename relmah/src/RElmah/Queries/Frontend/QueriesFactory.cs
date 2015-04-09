using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using RElmah.Domain;
using RElmah.Errors;
using RElmah.Extensions;
using RElmah.Foundation;
using RElmah.Models;
using RElmah.Notifiers;
using RElmah.Publishers;

namespace RElmah.Queries.Frontend
{
    public class QueriesFactory : IFrontendQueriesFactory
    {
        private readonly IErrorsInbox  _errorsInbox;
        private readonly IErrorsInbox _backendErrorsInbox;
        private readonly IErrorsBacklog _errorsBacklog;
        private readonly IDomainPublisher _domainPublisher;
        private readonly IDomainPersistor _domainPersistor;
        private readonly IFrontendNotifier _frontendNotifier;
        private readonly Func<IFrontendQuery>[] _subscriptors;

        private readonly AtomicImmutableDictionary<string, LayeredDisposable> _subscriptions = new AtomicImmutableDictionary<string, LayeredDisposable>();

        public QueriesFactory(IErrorsInbox errorsInbox, IErrorsInbox backendErrorsInbox, IErrorsBacklog errorsBacklog, IDomainPublisher domainPublisher, IDomainPersistor domainPersistor,  
            IFrontendNotifier frontendNotifier,
            params Func<IFrontendQuery>[] subscriptors)
        {
            _errorsInbox  = errorsInbox;
            _backendErrorsInbox = backendErrorsInbox;
            _errorsBacklog = errorsBacklog;
            _domainPublisher = domainPublisher;
            _domainPersistor = domainPersistor;
            _frontendNotifier = frontendNotifier;
            _subscriptors = subscriptors;
        }

        public async void Setup(string user, string token, Action<string> connector)
        {
            Func<IEnumerable<Source>> getUserSources = () => _domainPersistor.GetUserSources(user).Result;

            var ut = await _domainPersistor.AddUserToken(user, token);

            var subscriptions =
                from subscriptor in _subscriptors
                select subscriptor().Run(ut, new RunTargets
                {
                    FrontendNotifier = _frontendNotifier,
                    ErrorsInbox = _errorsInbox,
                    BackendErrorsInbox = _backendErrorsInbox,
                    ErrorsBacklog = _errorsBacklog,
                    DomainPersistor = _domainPersistor,
                    DomainPublisher = _domainPublisher
                });

            subscriptions = subscriptions.ToArray();
            await Task.WhenAll(subscriptions);

            var d = new CompositeDisposable(from s in subscriptions select s.Result).ToLayeredDisposable();

            _subscriptions.SetItem(user, d);

            //sources
            getUserSources().Do(s => connector(s.SourceId));
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