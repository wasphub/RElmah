using System;
using RElmah.Domain;
using RElmah.Errors;
using RElmah.Middleware.Bootstrapping.Builder;
using RElmah.Notifiers;
using RElmah.Publishers;
using RElmah.Queries;
using RElmah.Queries.Backend;
using RElmah.Queries.Frontend;
using RElmah.Services;
using RElmah.Services.Inbox;
using RElmah.Services.Nulls;
using QueriesFactory = RElmah.Queries.Frontend.QueriesFactory;

namespace RElmah.Middleware.Bootstrapping
{
    public static class Bootstrapper
    {
        public static T Prepare<T>(this IRegistry registry, IFrontendNotifier frontendNotifier, Func<string, IErrorsInbox, IDomainPersistor, IBackendNotifier> frontendBackendNotifierCreator, Func<IBackendNotifier> backendFrontendNotifierCreator, Func<IFrontendQueriesFactory, IBackendQueriesFactory, IErrorsInbox, IDomainPersistor, IBackendNotifier, T> resultor, IIdentityProvider identityProvider, BootstrapSettings settings)
        {
            var ebl = new InMemoryErrorsBacklog();
            var fei = new QueuedErrorsInbox(ebl);

            var bei = new QueuedErrorsInbox();   //for backend only

            var ds  = settings.DomainStoreBuilder != null
                    ? settings.DomainStoreBuilder()
                    : new InMemoryDomainStore();

            var dh  = new DomainHolder(ds);

            var fqf = new QueriesFactory(fei, bei, ebl, dh, dh, frontendNotifier,
                () => new ErrorsQuery(),
                () => new RecapsQuery());

            var ben = settings.Side == Side.Frontend && !string.IsNullOrWhiteSpace(settings.TargetBackendEndpoint)
                    ? frontendBackendNotifierCreator(settings.TargetBackendEndpoint, bei, dh)
                    : settings.Side == Side.Backend
                      ? backendFrontendNotifierCreator()
                      : NullBackendNotifier.Instance;

            var bqf = new Queries.Backend.QueriesFactory(fei, ebl, dh, dh, ben,
                () => new ErrorsBusQuery(),
                () => new ConfigurationBusQuery(settings.Side == Side.Backend));

            //Infrastructure
            registry.Register(typeof(IErrorsBacklog),          () => ebl);
            registry.Register(typeof(IErrorsInbox),            () => fei);
            registry.Register(typeof(IDomainPublisher),        () => dh);
            registry.Register(typeof(IDomainPersistor),        () => dh);
            registry.Register(typeof(IDomainStore),            () => ds);
            registry.Register(typeof(IFrontendQueriesFactory), () => fqf);
            registry.Register(typeof(IBackendQueriesFactory),  () => bqf);

            return resultor(fqf, bqf, fei, dh, ben);
        }
    }
}
