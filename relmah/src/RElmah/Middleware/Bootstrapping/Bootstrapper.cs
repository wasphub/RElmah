using System;
using RElmah.Visibility;
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

using FrontendErrorsQuery    = RElmah.Queries.Frontend.ErrorsQuery;
using BackendErrorsQuery     = RElmah.Queries.Backend.ErrorsQuery;
using FrontendQueriesFactory = RElmah.Queries.Frontend.QueriesFactory;
using BackendQueriesFactory  = RElmah.Queries.Backend.QueriesFactory;

namespace RElmah.Middleware.Bootstrapping
{
    public static class Bootstrapper
    {
        public static T Prepare<T>(this IRegistry registry, IFrontendNotifier frontendNotifier, Func<string, IErrorsInbox, IVisibilityPersistor, IBackendNotifier> frontendBackendNotifierCreator, Func<IBackendNotifier> backendFrontendNotifierCreator, Func<IFrontendQueriesFactory, IBackendQueriesFactory, IErrorsInbox, IVisibilityPersistor, IBackendNotifier, T> resultor, IIdentityProvider identityProvider, BootstrapSettings settings)
        {
            var ebl = new InMemoryErrorsBacklog();
            var fei = new QueuedErrorsInbox(ebl, 30, 1);

            var bei = new QueuedErrorsInbox();   //for backend only

            var ds  = settings.VisibilityStoreBuilder != null
                    ? settings.VisibilityStoreBuilder()
                    : new InMemoryVisibilityStore();

            var dh  = new VisibilityHolder(ds);

            var fqf = new FrontendQueriesFactory(fei, bei, ebl, ebl, dh, dh, frontendNotifier,
                () => new FrontendErrorsQuery(),
                () => new RecapsQuery());

            var ben = settings.Side == Side.Frontend && !string.IsNullOrWhiteSpace(settings.TargetBackendEndpoint)
                    ? frontendBackendNotifierCreator(settings.TargetBackendEndpoint, bei, dh)
                    : settings.Side == Side.Backend
                      ? backendFrontendNotifierCreator()
                      : NullBackendNotifier.Instance;

            var bqf = new BackendQueriesFactory(fei, ebl, ebl, dh, dh, ben,
                () => new BackendErrorsQuery(),
                () => new VisibilityQuery(settings.Side == Side.Frontend));

            //Infrastructure
            registry.Register(typeof(IErrorsBacklogWriter),          () => ebl);
            registry.Register(typeof(IErrorsInbox),            () => fei);
            registry.Register(typeof(IVisibilityPublisher),        () => dh);
            registry.Register(typeof(IVisibilityPersistor),        () => dh);
            registry.Register(typeof(IVisibilityStore),            () => ds);
            registry.Register(typeof(IFrontendQueriesFactory), () => fqf);
            registry.Register(typeof(IBackendQueriesFactory),  () => bqf);

            return resultor(fqf, bqf, fei, dh, ben);
        }
    }
}
