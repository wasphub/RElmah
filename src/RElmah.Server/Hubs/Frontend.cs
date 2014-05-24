using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace RElmah.Server.Hubs
{
    [HubName("relmah")]
    public class Frontend : Hub
    {
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
    }
}
