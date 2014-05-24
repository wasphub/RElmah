using System.Collections.Generic;

namespace RElmah.Domain
{
    public class Cluster
    {
        public Cluster(string name, IEnumerable<Application> applications)
        {
            Name = name;
            Applications = applications;
        }

        public Cluster(string name)
        {
            Name = name;
        }

        public Cluster()
        {
            
        }

        public string Name { get; set; }

        public IEnumerable<Application> Applications { get; set; }
    }
}
