using System;
using RElmah.Domain;
using RElmah.Errors;
using RElmah.Models.Settings;
using RElmah.Notifiers;
using RElmah.Publishers;
using RElmah.Services;
using RElmah.Services.Inbox;
using RElmah.Queries;
using RElmah.Queries.Backend;
using RElmah.Queries.Frontend;
using RElmah.Services.Nulls;

namespace RElmah.Middleware
{
    public static class Bootstrapper
    {
        public static T Prepare<T>(
            this IRegistry registry, 
            IFrontendNotifier frontendNotifier,
            Func<string, IErrorsInbox, IBackendNotifier> backendNotifierCreator, 
            IIdentityProvider identityProvider,
            Func<IFrontendQueriesFactory, IBackendQueriesFactory, IErrorsInbox, IDomainPersistor, IBackendNotifier, T> resultor,
            Settings settings = null)
        {
            var bl = new InMemoryErrorsBacklog();
            var ei = new QueuedErrorsInbox(bl);

            var ds = settings != null && settings.Domain != null && settings.Domain.DomainStoreBuilder != null
                   ? settings.Domain.DomainStoreBuilder()
                   : new InMemoryDomainStore();

            var dh = new DomainHolder(ds);

            var fqf = new FrontendQueriesFactory(ei, bl, dh, dh, frontendNotifier,
                     () => new ErrorsFrontendQuery(),
                     () => new RecapsFrontendQuery());

            var bqf = NullBackendQueriesFactory.Instance;
            var bn  = NullBackendNotifier.Instance;
            if (settings != null && settings.Bootstrap != null && !string.IsNullOrWhiteSpace(settings.Bootstrap.TargetBackendEndpoint))
            {
                bn  = backendNotifierCreator(settings.Bootstrap.TargetBackendEndpoint, ei);
                bqf = new BackendQueriesFactory(ei, bl, dh, dh, bn, () => new BackendBusQuery());
            }

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
    }
}
