using System;
using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Host.Hubs;
using RElmah.Host.Hubs.Notifiers;
using RElmah.Host.Services;
using RElmah.Middleware;
using RElmah.Middleware.Bootstrapping;
using RElmah.Middleware.Bootstrapping.Builder;
using RElmah.Models.Settings;
using RElmah.Notifiers;

namespace RElmah.Host.Extensions.AppBuilder
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmahObsolete(this IAppBuilder builder, Settings settings = null)
        {
            var registry = new Registry();

            var frontend = new FrontendNotifier();

            var ip       = settings != null && settings.Bootstrap != null && settings.Bootstrap.IdentityProviderBuilder != null
                         ? settings.Bootstrap.IdentityProviderBuilder()
                         : new WindowsPrincipalIdentityProvider();

            var bp       = registry.PrepareObsolete(frontend, 
                (ep, ei, dh) => new FrontendBackendNotifier(ep, ei, dh),
                () => new BackendFrontendNotifier(), 
                ip, 
                (fqf, bqf, ei, dh, bn) => new { fqf, bqf, ei, dh, bn }, 
                settings);

            var dp       = new DelegatingUserIdProvider(ip);

            registry.Register(typeof(IUserIdProvider), () => dp);

            //Frontend or Backend?
            if (settings != null && settings.Bootstrap != null && settings.Bootstrap.RunBackend)
                registry.Register(typeof(BackendHub), () => new BackendHub());
            else
                registry.Register(typeof(ErrorsHub), () => new ErrorsHub(bp.fqf, dp));

            if (settings != null && settings.Bootstrap != null && settings.Bootstrap.Registry != null)
                settings.Bootstrap.Registry(registry);

            if (settings != null && settings.Bootstrap != null && settings.Bootstrap.Domain != null)
                settings.Bootstrap.Domain(bp.dh);

            if (settings != null && settings.Errors != null)
                builder = settings.Errors.UseRandomizer
                        ? builder.UseRElmahMiddleware<RandomSourceErrorsMiddleware>(bp.ei, bp.dh, settings.Errors)
                        : builder.UseRElmahMiddleware<ErrorsMiddleware>(bp.ei, settings.Errors);

            if (settings != null && settings.Domain != null)
                builder = builder.UseRElmahMiddleware<DomainMiddleware>(bp.dh, settings.Domain);

            bp.bqf.Setup();
            
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

        public static IAppBuilder UseRElmah(this IAppBuilder builder, Func<BootstrapSettingsBuilder, BootstrapSettings> bootstrapper)
        {
            var settings = bootstrapper(new BootstrapSettingsBuilder());

            var registry = new Registry();

            var frontend = new FrontendNotifier();

            var ip = settings.IdentityProviderBuilder != null
                   ? settings.IdentityProviderBuilder()
                   : new WindowsPrincipalIdentityProvider();

            var bp = registry.Prepare(frontend,
                (ep, ei, dh) => new FrontendBackendNotifier(ep, ei, dh),
                () => new BackendFrontendNotifier(),
                ip,
                (fqf, bqf, ei, dh, bn) => new { fqf, bqf, ei, dh, bn },
                settings);

            var dp = new DelegatingUserIdProvider(ip);

            registry.Register(typeof(IUserIdProvider), () => dp);

            //Frontend or Backend?
            if (settings.Side == Side.Backend)
                registry.Register(typeof(BackendHub), () => new BackendHub());
            else
                registry.Register(typeof(ErrorsHub), () => new ErrorsHub(bp.fqf, dp));

            if (settings.RegistryConfigurator != null)
                settings.RegistryConfigurator(registry);

            if (settings.DomainConfigurator != null)
                settings.DomainConfigurator(bp.dh);

            builder = settings.ForErrors && settings.UseRandomizer
                ? builder.UseRElmahMiddleware<RandomSourceErrorsMiddleware>(bp.ei, bp.dh, new ErrorsSettings
                {
                    Prefix = settings.ErrorsPrefix
                })
                : builder.UseRElmahMiddleware<ErrorsMiddleware>(bp.ei, new ErrorsSettings
                {
                    Prefix = settings.ErrorsPrefix
                });

            if (settings.ForDomain)
                builder = builder.UseRElmahMiddleware<DomainMiddleware>(bp.dh, new DomainSettings
                {
                    Prefix = settings.DomainPrefix
                });

            bp.bqf.Setup();

            return builder;
        }
    }
}
