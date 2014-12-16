using System;
using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Host.Hubs;
using RElmah.Middleware;
using RElmah.Services;

namespace RElmah.Host
{
    public class Settings
    {
        public Action<IRegistry> BootstrapRegistry { get; set; }
        public Action<IConfigurationUpdater> BootstrapConfiguration { get; set; }
        public Func<IConfigurationStore> BuildConfigurationStore { get; set; }

        public bool ExposeConfigurationWebApi { get; set; }
    }

    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmah(this IAppBuilder builder, Settings settings = null)
        {
            var registry = new Registry();

            var ei       = new ErrorsInbox();
            var cs       = settings.SafeCall(
                             s  => s.BuildConfigurationStore(), 
                             () => new InMemoryConfigurationStore(), 
                             s  => s != null && s.BuildConfigurationStore != null);
                         
            var ch       = new ConfigurationHolder(cs);
            var c        = new Connector(ch);

            //Infrastructure
            registry.Register(typeof(IErrorsInbox),           () => ei);
            registry.Register(typeof(IConnection),            () => c);
            registry.Register(typeof(IConfigurationProvider), () => ch);
            registry.Register(typeof(IConfigurationUpdater),  () => ch);
            registry.Register(typeof(IConfigurationStore),    () => cs);

            //Hubs
            registry.Register(typeof(ErrorsHub), () => new ErrorsHub(c, registry.Resolve<IUserIdProvider>()));

            if (settings != null && settings.BootstrapRegistry != null)
                settings.BootstrapRegistry(registry);

            if (settings != null && settings.BootstrapConfiguration != null)
                settings.BootstrapConfiguration(ch);

            builder = builder.UseRElmahMiddleware<ErrorsMiddleware>(registry);
            if (settings != null && settings.ExposeConfigurationWebApi)
                builder = builder.UseRElmahMiddleware<ConfigurationMiddleware>(registry);

            //Init app streams
            Dispatcher.Wire(ei, ch);

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
