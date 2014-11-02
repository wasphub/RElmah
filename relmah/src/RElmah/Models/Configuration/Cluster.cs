using System.Collections.Generic;

namespace RElmah.Models.Configuration
{
    public class Cluster
    {
        private readonly IDictionary<string, Application> _applications = new Dictionary<string, Application>();
        private readonly IDictionary<string, User> _users = new Dictionary<string, User>();


        public string Name { get; private set; }
    }
}
