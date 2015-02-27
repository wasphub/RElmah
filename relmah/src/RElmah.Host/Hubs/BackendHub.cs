using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RElmah.Common;

namespace RElmah.Host.Hubs
{
    [HubName("relmah-backend")]
    public class BackendHub : Hub
    {
        public void Error(ErrorPayload payload)
        {
            Clients.Others.error(payload);
        }
    }
}
