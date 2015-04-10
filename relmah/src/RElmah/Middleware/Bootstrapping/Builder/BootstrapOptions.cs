using System;
using RElmah.Domain;

namespace RElmah.Middleware.Bootstrapping.Builder
{
    public class BootstrapOptions
    {
        public Func<Func<IIdentityProvider>> IdentityProviderBuilderSetter { get; set; }
        public Action<IRegistry> RegistryConfigurator { get; set; }

        public Func<IDomainStore> DomainStoreBuilder { get; set; }
    }
}