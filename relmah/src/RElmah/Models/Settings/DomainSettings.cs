using System;
using RElmah.Domain;

namespace RElmah.Models.Settings
{
    public class DomainSettings
    {
        public string Prefix { get; set; }
        public Func<IDomainStore> DomainStoreBuilder { get; set; }
    }
}