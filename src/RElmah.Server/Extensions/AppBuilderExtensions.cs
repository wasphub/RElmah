using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Server.Hubs;
using RElmah.Server.Middleware;
using RElmah.Server.Services;

namespace RElmah.Server.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmah(this IAppBuilder builder)
        {
            return builder.UseRElmah(new Configuration());
        }

        public static IAppBuilder UseRElmah(this IAppBuilder builder, Configuration configuration)
        {
            var registry = GlobalHost.DependencyResolver;

            var d  = new Dispatcher();
            var cp = new ConfigurationProvider(d);
            var ei = new ErrorsInbox(d);

            registry.Register(typeof(IDispatcher), () => d);
            registry.Register(typeof(IConfigurationProvider), () => cp);
            registry.Register(typeof(IErrorsInbox), () => ei);

            registry.Register(typeof(Frontend), () => new Frontend(cp));

            if (configuration.Register != null)
                configuration.Register(registry);

            return builder.UseRElmahMiddleware<RElmahMiddleware>(configuration, GlobalHost.DependencyResolver);
        }

        public static IAppBuilder RunSignalR(this IAppBuilder builder)
        {
            OwinExtensions.RunSignalR(builder);

            return builder;
        }

        static IAppBuilder UseRElmahMiddleware<T>(this IAppBuilder builder, params object[] args)
        {
            return builder.Use(typeof(T), args);
        }
    }
}
