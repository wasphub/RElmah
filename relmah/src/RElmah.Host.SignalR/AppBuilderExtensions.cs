using System;
using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Middleware;
using RElmah.Services;

namespace RElmah.Host.SignalR
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmah(this IAppBuilder builder)
        {
            var registry = GlobalHost.DependencyResolver;

            //TODO: improve the way this part can be customized from outside

            var ei = new ErrorsInbox();
            var cs = new InMemoryConfigurationStore();
            var ch = new ConfigurationHolder(cs);
            var d  = new Dispatcher(ei, ch);

            registry.Register(typeof(IErrorsInbox), () => ei);
            registry.Register(typeof(IDispatcher),  () => d);
            registry.Register(typeof(IConfigurationProvider), () => ch);
            registry.Register(typeof(IConfigurationUpdater), () => ch);
            registry.Register(typeof(IConfigurationStore), () => cs);

            registry.Register(typeof(IUserIdProvider), () => new ClientTokenUserIdProvider());

            return builder.UseRElmahMiddleware<RElmahMiddleware>(new Resolver());
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
