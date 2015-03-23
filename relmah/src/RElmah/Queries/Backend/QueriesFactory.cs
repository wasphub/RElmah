using System;
using RElmah.Domain;
using RElmah.Errors;
using RElmah.Extensions;
using RElmah.Notifiers;
using RElmah.Publishers;

namespace RElmah.Queries.Backend
{
    public class QueriesFactory : IBackendQueriesFactory
    {
        private readonly IErrorsInbox _errorsInbox;
        private readonly IErrorsBacklog _errorsBacklog;
        private readonly IDomainPublisher _domainPublisher;
        private readonly IDomainPersistor _domainPersistor;
        private readonly IBackendNotifier _backendNotifier;
        private readonly Func<IBackendQuery>[] _subscriptors;

        public QueriesFactory(IErrorsInbox errorsInbox, IErrorsBacklog errorsBacklog, IDomainPublisher domainPublisher, IDomainPersistor domainPersistor,
            IBackendNotifier backendNotifier,
            params Func<IBackendQuery>[] subscriptors)
        {
            _errorsInbox = errorsInbox;
            _errorsBacklog = errorsBacklog;
            _domainPublisher = domainPublisher;
            _domainPersistor = domainPersistor;
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
                    DomainPersistor = _domainPersistor,
                    DomainPublisher = _domainPublisher
                }));
        }
    }
}
