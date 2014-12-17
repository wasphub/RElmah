using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Host.Hubs;
using RElmah.Middleware;
using RElmah.Models.Settings;
using RElmah.Services;

namespace RElmah.Host.Extensions.AppBuilder
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmah(this IAppBuilder builder, Settings settings = null)
        {
            var registry = new Registry();

            var ei       = new ErrorsInbox();
            var cs       = settings.SafeCall(
                             s  => s.Domain.DomainStoreBuilder(), 
                             () => new InMemoryDomainStore(),
                             s => s != null && s.Domain != null && s.Domain.DomainStoreBuilder != null);
                         
            var ch       = new DomainHolder(cs);

            var c        = new Connector(ei, ch, ch);

            //Infrastructure
            registry.Register(typeof(IErrorsInbox),  () => ei);
            registry.Register(typeof(IConnection),   () => c);
            registry.Register(typeof(IDomainReader), () => ch);
            registry.Register(typeof(IDomainWriter), () => ch);
            registry.Register(typeof(IDomainStore),  () => cs);

            //Hubs
            registry.Register(typeof(ErrorsHub), () => new ErrorsHub(c, registry.Resolve<IUserIdProvider>()));

            if (settings != null && settings.Bootstrap.Registry != null)
                settings.Bootstrap.Registry(registry);

            if (settings != null && settings.Bootstrap.Domain != null)
                settings.Bootstrap.Domain(ch);

            if (settings != null && settings.Errors != null)
                builder = builder.UseRElmahMiddleware<ErrorsMiddleware>(registry, settings.Errors);
            if (settings != null && settings.Domain != null && settings.Domain.Exposed)
                builder = builder.UseRElmahMiddleware<DomainMiddleware>(registry, settings.Domain);

            //Init streams
            c.Start();

            return builder;
        }

        static IAppBuilder UseRElmahMiddleware<T>(this IAppBuilder builder, params object[] args)
        {
            return builder.Use(typeof(T), args);
        }

        public static IAppBuilder RunSignalR(this IAppBuilder builder)
        {
            OwinExtensions.RunSignalR(builder);

            return builder;
        }
    }
}
