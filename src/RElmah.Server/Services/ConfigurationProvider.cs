using System.Collections.Generic;
using RElmah.Server.Domain;

namespace RElmah.Server.Services
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public IEnumerable<Cluster> Clusters { get; private set; }
        public IList<string> GetGroups(ErrorPayload payload)
        {
            return new [] { "foo" };
        }
    }
}
