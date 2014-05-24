using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using RElmah.Domain;
using RElmah.Server.Hubs;
using RElmah.Server.Infrastructure;

namespace RElmah.Server.Services
{
    public class Dispatcher : IDispatcher
    {
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<Frontend>();

        public Task DispatchError(ErrorPayload payload)
        {
            return _context.Clients.All.error(payload);
        }

        public Task DispatchClusterAction(Operation<Cluster> cluster)
        {
            return _context.Clients.All.clusterUpdate(cluster);
        }
    }
}
