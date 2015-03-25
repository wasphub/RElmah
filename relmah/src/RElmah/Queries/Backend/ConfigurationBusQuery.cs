using System;
using System.Threading.Tasks;

namespace RElmah.Queries.Backend
{
    public class ConfigurationBusQuery : IBackendQuery
    {
        public async Task<IDisposable> Run(RunTargets targets)
        {
            return targets.DomainPublisher.GetClustersSequence().Subscribe(payload =>
            {
                targets.BackendNotifier.Cluster(payload);
            });
        }
    }
}