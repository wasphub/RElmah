using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;

namespace RElmah.Services.Nulls
{
    class NullConfigurationUpdater : IConfigurationUpdater
    {
        public Task<ValueOrError<Cluster>> AddCluster(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<bool>> RemoveCluster(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Cluster>> GetClusters()
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<Cluster>> GetCluster(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
