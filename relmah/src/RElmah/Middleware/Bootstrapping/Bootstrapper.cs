using System;
using RElmah.Domain;
using RElmah.Errors;
using RElmah.Middleware.Bootstrapping.Builder;
using RElmah.Models.Settings;
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
        public static T PrepareObsolete<T>(
            this IRegistry registry, 
            IFrontendNotifier frontendNotifier,
            Func<string, IErrorsInbox, IDomainPersistor, IBackendNotifier> frontendBackendNotifierCreator,
            Func<IBackendNotifier> backendFrontendNotifierCreator, 
            IIdentityProvider identityProvider,
            Func<IFrontendQueriesFactory, IBackendQueriesFactory, IErrorsInbox, IDomainPersistor, IBackendNotifier, T> resultor,
            Settings settings = null)
        {
            var bl = new InMemoryErrorsBacklog();
            var ei = new QueuedErrorsInbox(bl);

            var bi = new QueuedErrorsInbox();   //for backend only

            var ds = settings != null && settings.Domain != null && settings.Domain.DomainStoreBuilder != null
                   ? settings.Domain.DomainStoreBuilder()
                   : new InMemoryDomainStore();

            var dh = new DomainHolder(ds);

            var fqf = new QueriesFactory(ei, bi, bl, dh, dh, frontendNotifier,
                     () => new ErrorsQuery(),
                     () => new RecapsQuery());

            var bn  = NullBackendNotifier.Instance;

            if (settings != null && settings.Bootstrap != null && !string.IsNullOrWhiteSpace(settings.Bootstrap.TargetBackendEndpoint))
            {
                bn  = frontendBackendNotifierCreator(settings.Bootstrap.TargetBackendEndpoint, bi, dh);
            }
            else if (settings != null && settings.Bootstrap != null && settings.Bootstrap.RunBackend)
            {
                bn = backendFrontendNotifierCreator();
            }

            var bqf = new Queries.Backend.QueriesFactory(ei, bl, dh, dh, bn,
                () => new ErrorsBusQuery(),
                () => new ConfigurationBusQuery(skipEventsFromBackend: settings == null || settings.Bootstrap == null || settings.Bootstrap.RunBackend));

            //Infrastructure
            registry.Register(typeof(IErrorsBacklog),          () => bl);
            registry.Register(typeof(IErrorsInbox),            () => ei);
            registry.Register(typeof(IDomainPublisher),        () => dh);
            registry.Register(typeof(IDomainPersistor),        () => dh);
            registry.Register(typeof(IDomainStore),            () => ds);
            registry.Register(typeof(IFrontendQueriesFactory), () => fqf);
            registry.Register(typeof(IBackendQueriesFactory),  () => bqf);

            return resultor(fqf, bqf, ei, dh, bn);
        }

        public static T Prepare<T>(
            this IRegistry registry,
            IFrontendNotifier frontendNotifier,
            Func<string, IErrorsInbox, IDomainPersistor, IBackendNotifier> frontendBackendNotifierCreator,
            Func<IBackendNotifier> backendFrontendNotifierCreator,
            IIdentityProvider identityProvider,
            Func<IFrontendQueriesFactory, IBackendQueriesFactory, IErrorsInbox, IDomainPersistor, IBackendNotifier, T> resultor,
            BootstrapSettings settings)
        {
            var bl = new InMemoryErrorsBacklog();
            var ei = new QueuedErrorsInbox(bl);

            var bi = new QueuedErrorsInbox();   //for backend only

            var ds = settings.DomainStoreBuilder != null
                   ? settings.DomainStoreBuilder()
                   : new InMemoryDomainStore();

            var dh = new DomainHolder(ds);

            var fqf = new QueriesFactory(ei, bi, bl, dh, dh, frontendNotifier,
                     () => new ErrorsQuery(),
                     () => new RecapsQuery());

            var bn = NullBackendNotifier.Instance;

            if (settings.Side == Side.Frontend && !string.IsNullOrWhiteSpace(settings.TargetBackendEndpoint))
            {
                bn = frontendBackendNotifierCreator(settings.TargetBackendEndpoint, bi, dh);
            }
            
            if (settings.Side == Side.Backend)
            {
                bn = backendFrontendNotifierCreator();
            }

            var bqf = new Queries.Backend.QueriesFactory(ei, bl, dh, dh, bn,
                () => new ErrorsBusQuery(),
                () => new ConfigurationBusQuery(skipEventsFromBackend: settings.Side == Side.Backend));

            //Infrastructure
            registry.Register(typeof(IErrorsBacklog), () => bl);
            registry.Register(typeof(IErrorsInbox), () => ei);
            registry.Register(typeof(IDomainPublisher), () => dh);
            registry.Register(typeof(IDomainPersistor), () => dh);
            registry.Register(typeof(IDomainStore), () => ds);
            registry.Register(typeof(IFrontendQueriesFactory), () => fqf);
            registry.Register(typeof(IBackendQueriesFactory), () => bqf);

            return resultor(fqf, bqf, ei, dh, bn);
        }
    }
}
