using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using TinyIoC;

namespace RElmah.Server.Infrastructure
{
    public class TinyIocDependencyResolver : DefaultDependencyResolver, IDependencyRegistry
    {
        public override object GetService(Type serviceType)
        {
            return TinyIoCContainer.Current.CanResolve(serviceType)
                ? TinyIoCContainer.Current.Resolve(serviceType)
                : base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var b = TinyIoCContainer.Current.ResolveAll(serviceType).ToArray();
            return b.Any() ? b : base.GetServices(serviceType);
        }

        public IDependencyRegistry Register(Type serviceType, Type implementationType)
        {
            TinyIoCContainer.Current.Register(serviceType, implementationType);
            return this;
        }

        public IDependencyRegistry RegisterAsSingleton(Type serviceType, Type implementationType)
        {
            TinyIoCContainer.Current.Register(serviceType, implementationType).AsSingleton();
            return this;
        }
    }
}