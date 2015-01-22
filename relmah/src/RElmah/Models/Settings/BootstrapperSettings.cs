using System;

namespace RElmah.Models.Settings
{
    public class BootstrapperSettings
    {
        public Action<IRegistry> Registry { get; set; }
        public Action<IDomainPersistor> Domain { get; set; }

        public Func<IIdentityProvider> IdentityProviderBuilder { get; set; }
    }
}