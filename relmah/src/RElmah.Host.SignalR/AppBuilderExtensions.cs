using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Services;

namespace RElmah.Host.SignalR
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmah(this IAppBuilder builder)
        {
            var registry = GlobalHost.DependencyResolver;

            var ei = new ErrorsInbox();
            var d  = new Dispatcher(ei);

            registry.Register(typeof(IErrorsInbox), () => ei);
            registry.Register(typeof(IDispatcher),  () => d);

            registry.Register(typeof(IUserIdProvider), () => new ClientTokenUserIdProvider());

            return builder.UseRElmahMiddleware<RElmahMiddleware>(GlobalHost.DependencyResolver);
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
