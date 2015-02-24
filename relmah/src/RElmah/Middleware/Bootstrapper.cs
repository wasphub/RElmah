using System;
using RElmah.Domain;
using RElmah.Errors;
using RElmah.Models.Settings;
using RElmah.Notifiers;
using RElmah.Publishers;
using RElmah.Services;
using RElmah.Services.Inbox;
using RElmah.Queries;
using RElmah.Queries.Frontend;

namespace RElmah.Middleware
{
    public static class Bootstrapper
    {
        public static T Prepare<T>(
            this IRegistry registry, 
            IFrontendNotifier frontendNotifier, 
            IIdentityProvider identityProvider,
            Func<IFrontendQueriesFactory, IErrorsInbox, IDomainPersistor, T> resultor,
            Settings settings = null)
        {
            var bl = new InMemoryErrorsBacklog();
            var ei = new QueuedErrorsInbox(bl);

            var ds = settings != null && settings.Domain != null && settings.Domain.DomainStoreBuilder != null
                   ? settings.Domain.DomainStoreBuilder()
                   : new InMemoryDomainStore();

            var dh = new DomainHolder(ds);

            var qf = new FrontendQueriesFactory(ei, bl, dh, dh, frontendNotifier,
                     () => new ErrorsFrontendQuery(),
                     () => new RecapsFrontendQuery());

            //Infrastructure
            registry.Register(typeof(IErrorsBacklog),          () => bl);
            registry.Register(typeof(IErrorsInbox),            () => ei);
            registry.Register(typeof(IDomainPublisher),        () => dh);
            registry.Register(typeof(IDomainPersistor),        () => dh);
            registry.Register(typeof(IDomainStore),            () => ds);
            registry.Register(typeof(IFrontendQueriesFactory), () => qf);

            return resultor(qf, ei, dh);
        }
    }
}
