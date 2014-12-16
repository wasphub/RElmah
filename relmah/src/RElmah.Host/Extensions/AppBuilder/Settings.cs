using System;

namespace RElmah.Host.Extensions.AppBuilder
{
    public class Settings
    {
        public Settings()
        {
            Bootstrapper = new Bootstrapper();
        }

        public Bootstrapper Bootstrapper { get; set; }

        public Func<IDomainStore> BuildConfigurationStore { get; set; }
        public bool ExposeConfigurationWebApi { get; set; }
    }
}