using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Server.Domain;

namespace RElmah.Server.Services.Nulls
{
    public class NullDispatcher : IDispatcher
    {
        public Task DispatchError(IConfigurationProvider configurationProvider, ErrorPayload payload)
        {
            return Task.FromResult<object>(null);
        }

        public Task DispatchClusterOperation(IConfigurationProvider configurationProvider, Operation<Cluster> op)
        {
            return Task.FromResult<object>(null);
        }

        public Task DispatchApplicationOperation(IConfigurationProvider configurationProvider, Operation<Application> op)
        {
            return Task.FromResult<object>(null);
        }
    }
}
