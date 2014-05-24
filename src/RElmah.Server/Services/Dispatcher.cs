using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using RElmah.Domain;
using RElmah.Server.Hubs;

namespace RElmah.Server.Services
{
    public class Dispatcher : IDispatcher
    {
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<Frontend>();

        public Task DispatchError(ErrorPayload payload)
        {
            return _context.Clients.All.error(payload);
        }

        public Task DispatchClusterOperation(Operation<Cluster> op)
        {
            return _context.Clients.All.clusterOperation(op);
        }

        public Task DispatchApplicationOperation(Operation<Application> op)
        {
            return _context.Clients.All.applicationOperation(op);
        }
    }
}
