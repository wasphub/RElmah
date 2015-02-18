using System;
using RElmah.Models.Settings;
using RElmah.Services;
using RElmah.Services.Inbox;
using RElmah.StandingQueries;

namespace RElmah.Middleware
{
    public static class Bootstrapper
    {
        public static T Prepare<T>(
            this IRegistry registry, 
            INotifier notifier, 
            IIdentityProvider identityProvider,
            Func<IStandingQueriesFactory, IErrorsInbox, IDomainPersistor, T> resultor,
            Settings settings = null)
        {
            var ei = new QueuedErrorsInbox(new InMemoryErrorsBacklog());

            var ds = settings != null && settings.Domain != null && settings.Domain.DomainStoreBuilder != null
                   ? settings.Domain.DomainStoreBuilder()
                   : new InMemoryDomainStore();

            var dh = new DomainHolder(ds);

            var qf = new StandingQueriesFactory(ei, dh, dh, notifier,
                     () => new ErrorsStandingQuery(),
                     () => new RecapsStandingQuery());

            //Infrastructure
            registry.Register(typeof(IErrorsInbox),            () => ei);
            registry.Register(typeof(IDomainPublisher),        () => dh);
            registry.Register(typeof(IDomainPersistor),        () => dh);
            registry.Register(typeof(IDomainStore),            () => ds);
            registry.Register(typeof(IStandingQueriesFactory), () => qf);

            return resultor(qf, ei, dh);
        }
    }
}
