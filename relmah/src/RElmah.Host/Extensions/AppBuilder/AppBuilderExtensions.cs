using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Host.Hubs;
using RElmah.Host.Services;
using RElmah.Middleware;
using RElmah.Models.Settings;
using RElmah.Services;
using RElmah.Subscriptions;

namespace RElmah.Host.Extensions.AppBuilder
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmah(this IAppBuilder builder, Settings settings = null)
        {
            var registry = new Registry();

            var ip       = settings != null && settings.Bootstrap != null && settings.Bootstrap.IdentityProviderBuilder != null
                         ? settings.Bootstrap.IdentityProviderBuilder()
                         : new WindowsPrincipalIdentityProvider();

            var ei       = new SerializedErrorsInbox(new InMemoryErrorsBacklog());
            var cs       = settings != null && settings.Domain != null && settings.Domain.DomainStoreBuilder != null
                         ? settings.Domain.DomainStoreBuilder()
                         : new InMemoryDomainStore();
                         
            var ch       = new DomainHolder(cs);

            var n        = new Notifier();

            var c        = new SubscriptionFactory(ei, ch, ch, n,
                            () => new ErrorsSubscription(),
                            () => new RecapsSubscription());

            var dp       = new DelegatingUserIdProvider(ip);

            //Infrastructure
            registry.Register(typeof(IErrorsInbox),     () => ei);
            registry.Register(typeof(IConnection),      () => c);
            registry.Register(typeof(IDomainPublisher), () => ch);
            registry.Register(typeof(IDomainPersistor), () => ch);
            registry.Register(typeof(IDomainStore),     () => cs);
            registry.Register(typeof(IUserIdProvider),  () => dp);

            //Hubs
            registry.Register(typeof(ErrorsHub), () => new ErrorsHub(c, dp));

            if (settings != null && settings.Bootstrap != null && settings.Bootstrap.Registry != null)
                settings.Bootstrap.Registry(registry);

            if (settings != null && settings.Bootstrap != null && settings.Bootstrap.Domain != null)
                settings.Bootstrap.Domain(ch);

            if (settings != null && settings.Errors != null)
                builder = settings.Errors.UseRandomizer
                        ? builder.UseRElmahMiddleware<RandomSourceErrorsMiddleware>(ei, ch, settings.Errors)
                        : builder.UseRElmahMiddleware<ErrorsMiddleware>(ei, settings.Errors);
            if (settings != null && settings.Domain != null && settings.Domain.Exposed)
                builder = builder.UseRElmahMiddleware<DomainMiddleware>(ch, settings.Domain);

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
