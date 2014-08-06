using System.Collections.Generic;

namespace RElmah.Server.Domain
{
    public class Cluster
    {
        public Cluster(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
