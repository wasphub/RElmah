using System;

namespace RElmah.Models.Settings
{
    public class Settings
    {
        public Settings()
        {
            Bootstrap = new BootstrapperSettings();
            Errors       = new ErrorsSettings();
            Domain       = new DomainSettings();
        }
        public BootstrapperSettings Bootstrap { get; set; }
        public ErrorsSettings Errors { get; set; }
        public DomainSettings Domain { get; set; }

    }

    public class ErrorsSettings
    {
        public string Prefix { get; set; }
    }

    public class DomainSettings
    {
        public string Prefix { get; set; }
        public Func<IDomainStore> DomainStoreBuilder { get; set; }
        public bool Exposed { get; set; }
    }
}