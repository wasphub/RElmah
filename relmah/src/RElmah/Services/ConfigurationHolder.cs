using System;
using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;

namespace RElmah.Services
{
    public class ConfigurationHolder : IConfigurationProvider, IConfigurationUpdater
    {
        private readonly IConfigurationUpdater _configurationStore;

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

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveCluster(string name)
        {
            var s = await _configurationStore.RemoveCluster(name);

            return s;
        }
    }
}
