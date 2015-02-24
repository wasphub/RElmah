﻿using Microsoft.AspNet.SignalR;
using Owin;
using RElmah.Host.Hubs;
using RElmah.Host.Services;
using RElmah.Middleware;
using RElmah.Models.Settings;

namespace RElmah.Host.Extensions.AppBuilder
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmah(this IAppBuilder builder, Settings settings = null)
        {
            var registry = new Registry();

            var frontend = new FrontendNotifier();
            var backend  = new BackendNotifier();

            var ip       = settings != null && settings.Bootstrap != null && settings.Bootstrap.IdentityProviderBuilder != null
                         ? settings.Bootstrap.IdentityProviderBuilder()
                         : new WindowsPrincipalIdentityProvider();

            var bp       = registry.Prepare(frontend, backend, ip, (fqf, bqf, ei, dh) => new { fqf, bqf, ei, dh }, settings);

            var dp       = new DelegatingUserIdProvider(ip);

            registry.Register(typeof(IUserIdProvider), () => dp);
            registry.Register(typeof(ErrorsHub), () => new ErrorsHub(bp.fqf, dp));
            registry.Register(typeof(BackendHub), () => new BackendHub());

            if (settings != null && settings.Bootstrap != null && settings.Bootstrap.Registry != null)
                settings.Bootstrap.Registry(registry);

            if (settings != null && settings.Bootstrap != null && settings.Bootstrap.Domain != null)
                settings.Bootstrap.Domain(bp.dh);

            if (settings != null && settings.Errors != null)
                builder = settings.Errors.UseRandomizer
                        ? builder.UseRElmahMiddleware<RandomSourceErrorsMiddleware>(bp.ei, bp.dh, settings.Errors)
                        : builder.UseRElmahMiddleware<ErrorsMiddleware>(bp.ei, settings.Errors);

            if (settings != null && settings.Domain != null && settings.Domain.Exposed)
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
    }
}
