using System;

namespace RElmah.Models.Settings
{
    public class Settings
    {
        public Settings()
        {
            Bootstrapper = new Bootstrapper();
        }

        public string Prefix { get; set; }
        public Bootstrapper Bootstrapper { get; set; }

        public Func<IDomainStore> BuildConfigurationStore { get; set; }
        public bool ExposeConfigurationWebApi { get; set; }
    }
}