using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace RElmah.Host.Hubs
{
    [HubName("relmah-errors")]
    public class ErrorsHub : Hub
    {
        private readonly ISubscriptionFactory      _subscriptionFactory;
        private readonly IUserIdProvider _userIdProvider;

        public ErrorsHub(ISubscriptionFactory subscriptionFactory, IUserIdProvider userIdProvider)
        {
            _subscriptionFactory = subscriptionFactory;
            _userIdProvider = userIdProvider;
        }

        public override Task OnConnected()
        {
            var apps = Enumerable.Empty<string>();

            _subscriptionFactory.Subscribe(_userIdProvider.GetUserId(Context.Request), Context.ConnectionId, a =>
            {
                apps = apps.Concat(new[] { a });
                Groups.Add(Context.ConnectionId, a);
            });

            Clients.Caller
                .applications(apps);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _subscriptionFactory.Disconnect(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }
    }
}
