using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RElmah.Common;

namespace RElmah.Host.Hubs
{
    [HubName("relmah-errors")]
    public class ErrorsHub : Hub
    {
        private readonly ISubscriptionFactory _subscriptionFactory;
        private readonly IUserIdProvider _userIdProvider;

        public ErrorsHub(ISubscriptionFactory subscriptionFactory, IUserIdProvider userIdProvider)
        {
            _subscriptionFactory = subscriptionFactory;
            _userIdProvider      = userIdProvider;
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

        public static void Recap(string user, Models.Recap recap)
        {
            GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>().Clients.User(user).recap(recap);
        }

        public static void Error(string user, ErrorPayload payload)
        {
            GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>().Clients.User(user).error(payload);
        }

        public static void UserApplications(string user, IEnumerable<string> added, IEnumerable<string> removed)
        {
            GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>().Clients.User(user)
                .applications(added, removed);
        }

        public static void AddGroup(string token, string group)
        {
            GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>().Groups.Add(token, group);
        }

        public static void RemoveGroup(string token, string group)
        {
            GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>().Groups.Remove(token, group);
        }
    }
}
