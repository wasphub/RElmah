using System;
using Microsoft.AspNet.SignalR;

namespace RElmah.Host
{
    class Registry : IRegistry
    {
        public T Resolve<T>()
        {
            return GlobalHost.DependencyResolver.Resolve<T>();
        }

        public void Register<T>(Func<T> supplier)
        {
            Register(typeof (T), () => supplier());
        }

        public void Register(Type type, Func<object> supplier)
        {
            GlobalHost.DependencyResolver.Register(type, supplier);
        }
    }
}
