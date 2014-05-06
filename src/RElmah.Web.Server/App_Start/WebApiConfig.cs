using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dependencies;
using RElmah.Server;
using RElmah.Server.Services;
using RElmah.Web.Server.Controllers;
using TinyIoC;

namespace RElmah.Web.Server
{
    public class TinyIocDependencyResolver : IDependencyResolver
    {
        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return TinyIoCContainer.Current.CanResolve(serviceType)
                   ? TinyIoCContainer.Current.Resolve(serviceType)
                   : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return TinyIoCContainer.Current.ResolveAll(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }
    }

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            TinyIoCContainer.Current.Register(typeof(RElmahController));

            TinyIoCContainer.Current.Register(typeof(IErrorsInbox), typeof(ErrorsInbox)).AsSingleton();
            TinyIoCContainer.Current.Register(typeof(IErrorsDispatcher), typeof(ErrorsDispatcher)).AsSingleton();
            TinyIoCContainer.Current.Register(typeof(IConfigurationProvider), typeof(ConfigurationProvider)).AsSingleton();

            GlobalConfiguration.Configuration.DependencyResolver = new TinyIocDependencyResolver();

            //config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "PostApi",
                routeTemplate: "relmah/post-error",
                defaults: new { controller = "RElmah", action = "PostError" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "relmah/{action}",
                defaults: new {  controller = "RElmah" }
            );
        }
    }
}
