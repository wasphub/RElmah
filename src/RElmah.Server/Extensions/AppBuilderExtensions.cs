using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Server.Infrastructure;
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
            var registry = GlobalHost.DependencyResolver as IDependencyRegistry;

            registry.RegisterAsSingleton(typeof(IConfigurationProvider), typeof(ConfigurationProvider));

            registry.RegisterAsSingleton(typeof(IErrorsInbox), typeof(ErrorsInbox));
            registry.RegisterAsSingleton(typeof(IDispatcher),  typeof(Dispatcher));

            if (configuration.Register != null)
                configuration.Register(registry);

            return builder.UseRElmahMiddleware<RElmahMiddleware>(configuration, GlobalHost.DependencyResolver);
        }

        public static IAppBuilder RunSignalR(this IAppBuilder builder)
        {
            GlobalHost.DependencyResolver = new TinyIocDependencyResolver();

            OwinExtensions.RunSignalR(builder);

            return builder;
        }

        static IAppBuilder UseRElmahMiddleware<T>(this IAppBuilder builder, params object[] args)
        {
            return builder.Use(typeof(T), args);
        }
    }
}
