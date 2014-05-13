using System.Collections.Generic;
using RElmah.Domain;

namespace RElmah.Server.Services
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public IEnumerable<Cluster> Clusters { get; private set; }
        public IEnumerable<string> ExtractGroups(ErrorPayload payload)
        {
            return new [] { "foo" };
        }
    }
}
