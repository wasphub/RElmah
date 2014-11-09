using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace RElmah.Host.SignalR.Hubs
{
    [HubName("relmah-errors")]
    public class ErrorsHub : Hub
    {
        private readonly IConnector      _connector;
        private readonly IUserIdProvider _userIdProvider;

        public ErrorsHub(IConnector connector, IUserIdProvider userIdProvider)
        {
            _connector = connector;
            _userIdProvider = userIdProvider;
        }

        public override Task OnConnected()
        {
            var apps = Enumerable.Empty<string>();

            _connector.Connect(_userIdProvider.GetUserId(Context.Request), Context.ConnectionId, a =>
            {
                apps = apps.Concat(new[] { a });
                Groups.Add(Context.ConnectionId, a);
            });

            Clients.Caller
                .applications(apps);

            return base.OnConnected();
        }
    }
}
