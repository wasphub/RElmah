using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RElmah.Common;
using RElmah.Server.Domain;

namespace RElmah.Server.Hubs
{
    [HubName("relmah")]
    public class Frontend : Hub
    {
        private readonly IConfigurationProvider _configurationProvider;

        public Frontend(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public override Task OnConnected()
        {
            Clients.Caller.clusterOperation(    new Operation<Cluster>(    _configurationProvider.Clusters,     OperationType.Create));

            Clients.Caller.applicationOperation(new Operation<Application>(_configurationProvider.Applications, OperationType.Create));

            return base.OnConnected();
        }
    }
}
