﻿using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using RElmah.Services;

namespace RElmah.Host.SignalR
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRElmah(this IAppBuilder builder)
        {
            var registry = GlobalHost.DependencyResolver;

            registry.Register(typeof(IErrorsInbox), () => new ErrorsInbox());

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

    public class RElmahMiddleware : OwinMiddleware
    {
        public RElmahMiddleware(OwinMiddleware next, IDependencyResolver resolver)
            : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            return Next.Invoke(context);
        }
    }
}
