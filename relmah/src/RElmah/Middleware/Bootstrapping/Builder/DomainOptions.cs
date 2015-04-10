using System;
using RElmah.Domain;

namespace RElmah.Middleware.Bootstrapping.Builder
{
    public class DomainOptions
    {
        public Func<string> PrefixSetter { get; set; }
        public Action<IDomainPersistor> DomainConfigurator { get; set; }
    }
}