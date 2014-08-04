using System;
using Microsoft.AspNet.SignalR;

namespace RElmah.Server.Middleware
{
    public class Configuration
    {
        public string Root { get; set; }
        public Action<IDependencyResolver> Register { get; set; }
    }
}       