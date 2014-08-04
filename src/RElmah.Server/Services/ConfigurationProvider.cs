using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using RElmah.Common;
using RElmah.Server.Domain;
using RElmah.Server.Infrastructure;

namespace RElmah.Server.Services
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        readonly ReactiveDictionary<string, Cluster>        _clusters     = new ReactiveDictionary<string, Cluster>();
        readonly ReactiveDictionary<string, Application>    _applications = new ReactiveDictionary<string, Application>();
        readonly ConcurrentDictionary<string, ISet<string>> _visibility   = new ConcurrentDictionary<string, ISet<string>>();

        public T ExtractInfo<T>(ErrorPayload payload, Func<string, string, T> resultor)
        {
            var application = _applications.Values.FirstOrDefault(a => a.SourceId == payload.SourceId);
            return resultor(application.Name, application.Cluster.Name);
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

        public IEnumerable<Application> GetVisibleApplications(IPrincipal user)
        {
            return
                from h in _visibility.GetOrAdd(user.Identity.Name, s => new HashSet<string>())
                join a in _applications.Values on h equals a.Cluster.Name
                select a;
        }

        public void AddUserToCluster(string user, string cluster)
        {
            _visibility.GetOrAdd(user, s => new HashSet<string>()).Add(cluster);
        }

        public IObservable<Operation<Cluster>> GetObservableClusters()
        {
            return _clusters;
        }

        public IObservable<Operation<Application>> GetObservableApplications()
        {
            return _applications;
        }

    }
}
