using System;

namespace RElmah.Server.Middleware
{
    public class Configuration
    {
        public string Root { get; set; }
        public Action<IDependencyRegistry> Register { get; set; }
    }
}