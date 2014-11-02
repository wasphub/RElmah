using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace RElmah.Host.SignalR.Hubs
{
    [HubName("trelmah-errors")]
    public class ErrorsHub : Hub
    {
        public string Echo(string message)
        {
            return message;
        }
    }
}
