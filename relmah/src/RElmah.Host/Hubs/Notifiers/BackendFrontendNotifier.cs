using Microsoft.AspNet.SignalR;
using RElmah.Common;
using RElmah.Foundation;
using RElmah.Models;
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

        public void Application(Delta<Application> payload)
        {
            _context.Clients.All.Application(payload);
        }

        public void User(Delta<User> payload)
        {
            _context.Clients.All.User(payload);
        }

        public void ClusterApplication(Delta<Relationship<Cluster, Application>> payload)
        {
            _context.Clients.All.ClusterApplication(payload);
        }

        public void ClusterUser(Delta<Relationship<Cluster, User>> payload)
        {
            _context.Clients.All.ClusterUser(payload);
        }
    }
}