using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;

namespace RElmah.Services
{
    public class ConfigurationHolder : IConfigurationProvider, IConfigurationUpdater
    {
        private readonly IConfigurationUpdater _configurationStore;

        private readonly Subject<Delta<Cluster>> _clusterOperations = new Subject<Delta<Cluster>>();

        public ConfigurationHolder(IConfigurationUpdater configurationStore)
        {
            _configurationStore = configurationStore;
        }

        public IObservable<Delta<Cluster>> ObserveClusters()
        {
            throw new NotImplementedException();
        }

        public async Task<ValueOrError<Cluster>> AddCluster(string name)
        {
            var s = await _configurationStore.AddCluster(name);

            if (s.HasValue) _clusterOperations.OnNext(Delta.Create(s.Value, DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveCluster(string name)
        {
            var s = await _configurationStore.RemoveCluster(name);

            if (s.HasValue) _clusterOperations.OnNext(Delta.Create(Cluster.Create(name), DeltaType.Removed));

            return s;
        }
    }
}
