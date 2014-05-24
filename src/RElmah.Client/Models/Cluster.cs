using System.Collections.Generic;

namespace RElmah.Client.Models
{
    public class Cluster
    {
        public string Name;

        public IEnumerable<Application> Applications;
    }
}