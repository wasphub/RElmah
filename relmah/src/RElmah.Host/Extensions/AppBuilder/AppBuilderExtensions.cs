using System;
using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Host.Hubs;
using RElmah.Host.Hubs.Notifiers;
using RElmah.Host.Services;
using RElmah.Middleware;
using RElmah.Middleware.Bootstrapping;
using RElmah.Middleware.Bootstrapping.Builder;

namespace RElmah.Host.Extensions.AppBuilder
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmah(this IAppBuilder builder, Func<BootstrapSettingsBuilder, BootstrapSettings> bootstrapper)
        {
            var settings = bootstrapper(new BootstrapSettingsBuilder());

            var registry = new Registry();
            if (settings.InitRegistry != null)
                settings.InitRegistry(registry);

            var frontend = new FrontendNotifier();

            var ip = settings.IdentityProviderBuilder != null
                   ? settings.IdentityProviderBuilder()
                   : new WindowsPrincipalIdentityProvider();

            var bp = registry.Prepare(
                frontend,
                (ep, ei, dh)           => new FrontendBackendNotifier(ep, ei, dh),
                ()                     => new BackendFrontendNotifier(),
                (fqf, bqf, ei, dh, bn) => new { fqf, bqf, ei, dh, bn },
                ip, settings);

            var dp = new DelegatingUserIdProvider(ip);

            registry.Register(typeof(IUserIdProvider), () => dp);

            //Frontend or Backend?
            if (settings.Side == Side.Backend)
                registry.Register(typeof(BackendHub), () => new BackendHub());
            else
                registry.Register(typeof(ErrorsHub), () => new ErrorsHub(bp.fqf, dp));

            if (settings.InitRegistry != null)
                settings.InitRegistry(registry);

            if (settings.InitConfiguration != null)
                settings.InitConfiguration(bp.dh);

            if (settings.ForErrors)
                builder = settings.UseRandomizer
                    ? builder.UseRElmahMiddleware<RandomSourceErrorsMiddleware>(bp.ei, bp.dh, settings.ErrorsPrefix)
                    : builder.UseRElmahMiddleware<ErrorsMiddleware>(bp.ei, settings.ErrorsPrefix);

            if (settings.ForVisibility)
                builder = builder.UseRElmahMiddleware<VisibilityMiddleware>(bp.dh, settings.VisibilityPrefix);

            bp.bqf.Setup();

            return builder;
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
