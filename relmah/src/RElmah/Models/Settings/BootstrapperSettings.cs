using System;

namespace RElmah.Models.Settings
{
    public class BootstrapperSettings
    {
        public Action<IRegistry> Registry { get; set; }
        public Action<IDomainWriter> Domain { get; set; }
    }
}