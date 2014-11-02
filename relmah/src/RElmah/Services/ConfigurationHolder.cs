using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;
using RElmah.Services.Nulls;

namespace RElmah.Services
{
    public class ConfigurationHolder : IConfigurationProvider, IConfigurationUpdater
    {
        private readonly IConfigurationUpdater _configurationStore;

        private readonly Subject<Delta<Cluster>> _clusterDeltas = new Subject<Delta<Cluster>>();

        public ConfigurationHolder(IConfigurationUpdater configurationStore)
        {
            _configurationStore = configurationStore;
        }

        public ConfigurationHolder() : this(new NullConfigurationUpdater()) { }

        public IObservable<Delta<Cluster>> ObserveClusters()
        {
            return _clusterDeltas;
        }

        public async Task<ValueOrError<Cluster>> AddCluster(string name)
        {
            var s = await _configurationStore.AddCluster(name);

            if (s.HasValue) _clusterDeltas.OnNext(Delta.Create(s.Value, DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveCluster(string name)
        {
            var s = await _configurationStore.RemoveCluster(name);

            if (s.HasValue) _clusterDeltas.OnNext(Delta.Create(Cluster.Create(name), DeltaType.Removed));

            return s;
        }

        public Task<IEnumerable<Cluster>> GetClusters()
        {
            return _configurationStore.GetClusters();
        }

        public Task<ValueOrError<Cluster>> GetCluster(string name)
        {
            return _configurationStore.GetCluster(name);
        }
    }
}
