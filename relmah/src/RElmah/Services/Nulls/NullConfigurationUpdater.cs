using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;

namespace RElmah.Services.Nulls
{
    class NullConfigurationUpdater : IConfigurationUpdater
    {
        public Task<ValueOrError<Cluster>> AddCluster(string name)
        {
            return Task.FromResult((ValueOrError<Cluster>)null);
        }

        public Task<ValueOrError<bool>> RemoveCluster(string name)
        {
            return Task.FromResult((ValueOrError<bool>)null);
        }
    }
}
