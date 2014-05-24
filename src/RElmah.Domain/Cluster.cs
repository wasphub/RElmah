using System.Collections.Generic;

namespace RElmah.Domain
{
    public class Cluster
    {
        public string Name { get; set; }

        public IEnumerable<Application> Applications { get; set; }
    }
}
