using Microsoft.AspNet.SignalR;
using RElmah.Common;
using RElmah.Common.Model;
using RElmah.Foundation;
using RElmah.Notifiers;

namespace RElmah.Host.Hubs.Notifiers
{
    public class BackendFrontendNotifier : IBackendNotifier
    {
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<BackendHub>();

        public void Error(ErrorPayload payload)
        {
            _context.Clients.All.Error(payload);
        }

        public void Cluster(Delta<Cluster> payload)
        {
            _context.Clients.All.Cluster(payload);
        }

        public void Source(Delta<Source> payload)
        {
            _context.Clients.All.Source(payload);
        }

        public void User(Delta<User> payload)
        {
            _context.Clients.All.User(payload);
        }

        public void ClusterSource(Delta<Relationship<Cluster, Source>> payload)
        {
            _context.Clients.All.ClusterSource(payload);
        }

        public void ClusterUser(Delta<Relationship<Cluster, User>> payload)
        {
            _context.Clients.All.ClusterUser(payload);
        }
    }
}