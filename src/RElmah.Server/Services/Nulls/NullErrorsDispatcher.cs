using System.Threading.Tasks;
using RElmah.Domain;
using RElmah.Server.Infrastructure;

namespace RElmah.Server.Services.Nulls
{
    public class NullDispatcher : IDispatcher
    {
        public Task DispatchError(ErrorPayload payload)
        {
            return Task.FromResult<object>(null);
        }

        public Task DispatchClusterAction(Operation<Cluster> cluster)
        {
            return Task.FromResult<object>(null);
        }
    }
}
