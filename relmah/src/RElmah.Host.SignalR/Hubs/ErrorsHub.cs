using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace RElmah.Host.SignalR.Hubs
{
    [HubName("relmah-errors")]
    public class ErrorsHub : Hub
    {
        private readonly IDispatcher _dispatcher;
        private readonly IUserIdProvider _userIdProvider;

        public ErrorsHub(IDispatcher dispatcher, IUserIdProvider userIdProvider)
        {
            _dispatcher = dispatcher;
            _userIdProvider = userIdProvider;
        }

        public override Task OnConnected()
        {
            _dispatcher.Connect( _userIdProvider.GetUserId(Context.Request), a => Groups.Add(Context.ConnectionId, a));

            return base.OnConnected();
        }

        public void Monitor(IEnumerable<string> apps)
        {
            foreach (var app in apps)
                Groups.Add(Context.ConnectionId, app);
        }
    }
}
