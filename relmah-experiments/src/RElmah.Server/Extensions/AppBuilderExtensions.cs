using System;
using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Server.Hubs;
using RElmah.Server.Middleware;
using RElmah.Server.Services;

namespace RElmah.Server.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmah(this IAppBuilder builder, Action<IConfigurationProvider> configurator = null)
        {
            return builder.UseRElmah(new Configuration(), configurator);
        }

        public static IAppBuilder UseRElmah(this IAppBuilder builder, Configuration configuration, Action<IConfigurationProvider> configurator = null)
        {
            var registry = GlobalHost.DependencyResolver;

            var cp = new ConfigurationProvider();
            var eb = new ErrorsBacklog();
            var ei = new ErrorsInbox(eb);
            var d = new Dispatcher(ei, cp);

            registry.Register(typeof(IDispatcher), () => d);
            registry.Register(typeof(IConfigurationProvider), () => cp);
            registry.Register(typeof(IErrorsInbox), () => ei);
            registry.Register(typeof(IErrorsBacklog), () => eb);

            registry.Register(typeof(Frontend), () => new Frontend(cp, eb));

            if (configuration.Register != null)
                configuration.Register(registry);

            if (configurator != null)
                configurator(cp);

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
