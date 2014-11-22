using Microsoft.AspNet.SignalR;

namespace RElmah.Host
{
    class Resolver : IResolver
    {
        public T Resolve<T>()
        {
            return GlobalHost.DependencyResolver.Resolve<T>();
        }
    }
}
