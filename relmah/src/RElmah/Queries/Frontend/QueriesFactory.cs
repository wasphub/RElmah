using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using RElmah.Common.Extensions;
using RElmah.Common.Model;
using RElmah.Errors;
using RElmah.Extensions;
using RElmah.Foundation;
using RElmah.Notifiers;
using RElmah.Publishers;
using RElmah.Visibility;

namespace RElmah.Queries.Frontend
{
    public class QueriesFactory : IFrontendQueriesFactory
    {
        private readonly IErrorsInbox  _errorsInbox;
        private readonly IErrorsInbox _backendErrorsInbox;
        private readonly IErrorsBacklogWriter _errorsBacklog;
        private readonly IErrorsBacklogReader _errorsBacklogReader;
        private readonly IVisibilityPublisher _visibilityPublisher;
        private readonly IVisibilityPersistor _visibilityPersistor;
        private readonly IFrontendNotifier _frontendNotifier;
        private readonly Func<IFrontendQuery>[] _subscriptors;

        private readonly AtomicImmutableDictionary<string, LayeredDisposable> _subscriptions = new AtomicImmutableDictionary<string, LayeredDisposable>();

        public QueriesFactory(IErrorsInbox errorsInbox, IErrorsInbox backendErrorsInbox, IErrorsBacklogWriter errorsBacklog, IErrorsBacklogReader errorsBacklogReader,  IVisibilityPublisher visibilityPublisher, IVisibilityPersistor visibilityPersistor,  
            IFrontendNotifier frontendNotifier,
            params Func<IFrontendQuery>[] subscriptors)
        {
            _errorsInbox  = errorsInbox;
            _backendErrorsInbox = backendErrorsInbox;
            _errorsBacklog = errorsBacklog;
            _errorsBacklogReader = errorsBacklogReader;
            _visibilityPublisher = visibilityPublisher;
            _visibilityPersistor = visibilityPersistor;
            _frontendNotifier = frontendNotifier;
            _subscriptors = subscriptors;
        }

        public async void Setup(string user, string token, Action<string> connector)
        {
            Func<IEnumerable<Source>> getUserSources = () => _visibilityPersistor.GetUserSources(user).Result;

            var ut = await _visibilityPersistor.AddUserToken(user, token);

            var subscriptions =
                from subscriptor in _subscriptors
                select subscriptor().Run(ut, new RunTargets
                {
                    FrontendNotifier = _frontendNotifier,
                    FrontendErrorsInbox = _errorsInbox,
                    BackendErrorsInbox = _backendErrorsInbox,
                    ErrorsBacklog = _errorsBacklog,
                    ErrorsBacklogReader = _errorsBacklogReader,
                    VisibilityPersistor = _visibilityPersistor,
                    VisibilityPublisher = _visibilityPublisher
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
            var u = await _visibilityPersistor.RemoveUserToken(token);
            if (!u.HasValue) return;

            var name         = u.Value.Name;
            var subscription = _subscriptions.Get(name);

            if (!subscription.IsDisposed)
                subscription.Dispose();

            if (subscription.IsDisposed)
                _subscriptions.Remove(name);
        }       
    }
}