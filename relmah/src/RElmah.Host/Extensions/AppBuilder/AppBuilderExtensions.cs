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
                             s  => s.BuildConfigurationStore(), 
                             () => new InMemoryDomainStore(), 
                             s  => s != null && s.BuildConfigurationStore != null);
                         
            var ch       = new DomainHolder(cs);
            var c        = new Connector(ch);

            //Infrastructure
            registry.Register(typeof(IErrorsInbox),  () => ei);
            registry.Register(typeof(IConnection),   () => c);
            registry.Register(typeof(IDomainReader), () => ch);
            registry.Register(typeof(IDomainWriter), () => ch);
            registry.Register(typeof(IDomainStore),  () => cs);

            //Hubs
            registry.Register(typeof(ErrorsHub), () => new ErrorsHub(c, registry.Resolve<IUserIdProvider>()));

            if (settings != null && settings.Bootstrapper.Registry != null)
                settings.Bootstrapper.Registry(registry);

            if (settings != null && settings.Bootstrapper.Configuration != null)
                settings.Bootstrapper.Configuration(ch);

            builder = builder.UseRElmahMiddleware<ErrorsMiddleware>(registry, settings);
            if (settings != null && settings.ExposeConfigurationWebApi)
                builder = builder.UseRElmahMiddleware<DomainMiddleware>(registry);

            //Init app streams
            Observervations.Start(ei, ch);

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
