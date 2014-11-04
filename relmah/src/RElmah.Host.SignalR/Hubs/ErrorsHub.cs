using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace RElmah.Host.SignalR.Hubs
{
    [HubName("relmah-errors")]
    public class ErrorsHub : Hub
    {
        public void Monitor(IEnumerable<string> apps)
        {
            foreach (var app in apps)
                Groups.Add(Context.ConnectionId, app);
        }
    }
}
