using System;

namespace RElmah.Host.Extensions.AppBuilder
{
    public class Bootstrapper
    {
        public Action<IRegistry> Registry { get; set; }
        public Action<IDomainWriter> Configuration { get; set; }
    }
}