using System;
using RElmah.Visibility;

namespace RElmah.Middleware.Bootstrapping.Builder
{
    public class BootstrapSettings
    {
        internal BootstrapSettings() { }

        public Func<IIdentityProvider> IdentityProviderBuilder { get; internal set; }
        public string TargetBackendEndpoint { get; internal set; }
        public Side Side { get; internal set; }
        public bool ForErrors { get; internal set; }
        public bool ForVisibility { get; internal set; }
        public string ErrorsPrefix { get; internal set; }
        public string VisibilityPrefix { get; internal set; }
        public Func<IVisibilityStore> VisibilityStoreBuilder { get; internal set; }
        public Action<IVisibilityPersistor> InitVisibility { get; internal set; }
        public Action<IRegistry> InitRegistry { get; internal set; }
        public bool UseRandomizer { get; internal set; }
    }
}