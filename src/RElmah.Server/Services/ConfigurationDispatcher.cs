using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using RElmah.Domain;
using RElmah.Server.Hubs;

namespace RElmah.Server.Services
{
    public class ConfigurationDispatcher : IConfigurationDispatcher
    {
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<Frontend>();

        public Task Dispatch(Cluster cluster)
        {
            return _context.Clients.All.cluster(cluster);
        }
    }
}
