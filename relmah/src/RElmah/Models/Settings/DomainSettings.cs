using System;

namespace RElmah.Models.Settings
{
    public class DomainSettings
    {
        public string Prefix { get; set; }
        public Func<IDomainStore> DomainStoreBuilder { get; set; }
        public bool Exposed { get; set; }
    }
}