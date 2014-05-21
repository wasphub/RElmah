using System;
using System.Collections.Generic;
using RElmah.Domain;
using RElmah.Server.Infrastructure;

namespace RElmah.Server.Services
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        readonly ReactiveDictionary<string, Cluster> _clusters = new ReactiveDictionary<string, Cluster>();

        public ConfigurationProvider(IDispatcher dispatcher)
        {
            _clusters.Subscribe(p => dispatcher.DispatchClusterAction(p));
        }

        public IEnumerable<string> ExtractGroups(ErrorPayload payload)
        {
            return new [] { "foo" };
        }

        public IEnumerable<Cluster> Clusters { get { return _clusters.Values; } }

        public void AddCluster(Cluster cluster)
        {
            _clusters[cluster.Name] = cluster;
        }

        public Cluster GetCluster(string name)
        {
            return _clusters[name];
        }
    }
}
