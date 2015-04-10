using System;
using RElmah.Domain;

namespace RElmah.Middleware.Bootstrapping.Builder
{
    public class BootstrapSettings
    {
        internal BootstrapSettings() { }

        public Func<IIdentityProvider> IdentityProviderBuilder { get; set; }
        public string TargetBackendEndpoint { get; set; }
        public Side Side { get; set; }
        public bool ForErrors { get; set; }
        public bool ForDomain { get; set; }
        public string ErrorsPrefix { get; set; }
        public string DomainPrefix { get; set; }
        public Func<IDomainStore> DomainStoreBuilder { get; set; } 
        public Action<IDomainPersistor> DomainConfigurator { get; set; }
        public Action<IRegistry> RegistryConfigurator { get; set; }
        public bool UseRandomizer { get; set; }
    }
}