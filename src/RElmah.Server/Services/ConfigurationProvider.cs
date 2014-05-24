using System;
using System.Collections.Generic;
using RElmah.Server.Domain;
using RElmah.Server.Infrastructure;

namespace RElmah.Server.Services
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        readonly ReactiveDictionary<string, Cluster>     _clusters     = new ReactiveDictionary<string, Cluster>();
        readonly ReactiveDictionary<string, Application> _applications = new ReactiveDictionary<string, Application>();

        public ConfigurationProvider(IDispatcher dispatcher)
        {
            _clusters.Subscribe(p => dispatcher.DispatchClusterOperation(p));
            _applications.Subscribe(p => dispatcher.DispatchApplicationOperation(p));
        }

        public IEnumerable<Cluster> Clusters { get { return _clusters.Values; } }
        public IEnumerable<Application> Applications { get { return _applications.Values; } }

        public void AddCluster(string cluster)
        {
            _clusters[cluster] = new Cluster(cluster);
        }

        public Cluster GetCluster(string name)
        {
            return _clusters[name];
        }

        public void AddApplication(string name, string sourceId, string cluster)
        {
            if (!_clusters.ContainsKey(cluster))
                throw new ApplicationException("Must specify a valid and registered cluster");

            _applications[name] = new Application(name, sourceId, _clusters[cluster]);
        }

        public Application GetApplication(string name)
        {
            return _applications[name];
        }

        public IEnumerable<string> ExtractGroups(ErrorPayload payload)
        {
            return new[] { payload.SourceId };
        }
    }
}
