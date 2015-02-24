using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RElmah.Common;

namespace RElmah.Host.Hubs
{
    [HubName("relmah-backend")]
    public class BackendHub : Hub
    {
        public static void Error(ErrorPayload payload)
        {
            GlobalHost.ConnectionManager.GetHubContext<BackendHub>().Clients.All.error(payload);
        }
    }
}
