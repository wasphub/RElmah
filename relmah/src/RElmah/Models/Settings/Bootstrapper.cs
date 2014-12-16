using System;

namespace RElmah.Models.Settings
{
    public class Bootstrapper
    {
        public Action<IRegistry> Registry { get; set; }
        public Action<IDomainWriter> Configuration { get; set; }
    }
}