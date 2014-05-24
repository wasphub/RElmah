using System;
using System.Collections.Generic;
using System.Linq;
using RElmah.Domain;
using RElmah.Server.Extensions;
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

        public void AddCluster(Cluster cluster)
        {
            _clusters[cluster.Name] = cluster;
        }

        public Cluster GetCluster(string name)
        {
            return _clusters[name];
        }

        public void AddApplication(Application application)
        {
            if (application.Cluster == null || !_clusters.ContainsKey(application.Cluster.Name))
                throw new ApplicationException("application must specify a valid and registered cluster");

            _applications[application.Name] = application;
            _clusters[application.Cluster.Name].Applications = _clusters[application.Cluster.Name].Applications.Concat(application.ToSingleton());
        }

        public Application GetApplication(string name)
        {
            return _applications[name];
        }

        public IEnumerable<string> ExtractGroups(ErrorPayload payload)
        {
            return new[] { "foo" };
        }

    }
}
