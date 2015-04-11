using System;
using RElmah.Common.Extensions;
using RElmah.Errors;
using RElmah.Notifiers;
using RElmah.Publishers;
using RElmah.Visibility;

namespace RElmah.Queries.Backend
{
    public class QueriesFactory : IBackendQueriesFactory
    {
        private readonly IErrorsInbox _errorsInbox;
        private readonly IErrorsBacklog _errorsBacklog;
        private readonly IVisibilityPublisher _visibilityPublisher;
        private readonly IVisibilityPersistor _visibilityPersistor;
        private readonly IBackendNotifier _backendNotifier;
        private readonly Func<IBackendQuery>[] _subscriptors;

        public QueriesFactory(IErrorsInbox errorsInbox, IErrorsBacklog errorsBacklog, IVisibilityPublisher visibilityPublisher, IVisibilityPersistor visibilityPersistor,
            IBackendNotifier backendNotifier,
            params Func<IBackendQuery>[] subscriptors)
        {
            _errorsInbox = errorsInbox;
            _errorsBacklog = errorsBacklog;
            _visibilityPublisher = visibilityPublisher;
            _visibilityPersistor = visibilityPersistor;
            _backendNotifier = backendNotifier;
            _subscriptors = subscriptors;
        }

        public void Setup()
        {
            _subscriptors.Do(
                subscriptor => subscriptor().Run(new RunTargets
                {
                    BackendNotifier = _backendNotifier,
                    ErrorsInbox = _errorsInbox,
                    ErrorsBacklog = _errorsBacklog,
                    VisibilityPersistor = _visibilityPersistor,
                    VisibilityPublisher = _visibilityPublisher
                }));
        }
    }
}
