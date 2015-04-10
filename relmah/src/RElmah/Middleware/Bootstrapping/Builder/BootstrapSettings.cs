using System;
using RElmah.Domain;

namespace RElmah.Middleware.Bootstrapping.Builder
{
    public class BootstrapSettings
    {
        internal BootstrapSettings() { }

        public Func<IIdentityProvider> IdentityProviderBuilder { get; internal set; }
        public string TargetBackendEndpoint { get; internal set; }
        public Side Side { get; internal set; }
        public bool ForErrors { get; internal set; }
        public bool ForDomain { get; internal set; }
        public string ErrorsPrefix { get; internal set; }
        public string DomainPrefix { get; internal set; }
        public Func<IDomainStore> DomainStoreBuilder { get; internal set; }
        public Action<IDomainPersistor> InitConfiguration { get; internal set; }
        public Action<IRegistry> InitRegistry { get; internal set; }
        public bool UseRandomizer { get; internal set; }
    }
}