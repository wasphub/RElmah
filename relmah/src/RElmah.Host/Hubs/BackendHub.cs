using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RElmah.Common;
using RElmah.Models;

namespace RElmah.Host.Hubs
{
    [HubName("relmah-backend")]
    public class BackendHub : Hub
    {
        public void Error(ErrorPayload payload)
        {
            Clients.Others.error(payload);
        }

        public void Cluster(Cluster cluster, DeltaType type)
        {
            Clients.Others.cluster(cluster.Name);
        }
    }
}
