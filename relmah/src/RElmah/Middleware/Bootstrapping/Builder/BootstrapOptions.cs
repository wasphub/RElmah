using System;
using RElmah.Visibility;

namespace RElmah.Middleware.Bootstrapping.Builder
{
    public class BootstrapOptions
    {
        public Func<Func<IIdentityProvider>> IdentityProviderBuilderSetter { get; set; }
        public Action<IRegistry> InitRegistry { get; set; }

        public Func<IVisibilityStore> VisibilityStoreBuilder { get; set; }
        public Action<IVisibilityPersistor> InitConfiguration { get; set; }
    }
}