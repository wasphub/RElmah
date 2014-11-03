using Microsoft.AspNet.SignalR;

namespace RElmah.Host.SignalR
{
    class Resolver : IResolver
    {
        public T Resolve<T>()
        {
            return GlobalHost.DependencyResolver.Resolve<T>();
        }
    }
}
