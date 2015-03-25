using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RElmah.Common;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Host.Hubs
{
    [HubName("relmah-backend")]
    public class BackendHub : Hub
    {
        public void Error(ErrorPayload payload)
        {
            Clients.Others.error(payload);
        }

        public void Cluster(Delta<Cluster> cluster)
        {
            Clients.Others.cluster(cluster);
        }

        public void Application(Delta<Application> application)
        {
            Clients.Others.application(application);
        }

        public void User(Delta<User> user)
        {
            Clients.Others.user(user);
        }

        public void ClusterApplication(Delta<Relationship<Cluster, Application>> ca)
        {
            Clients.Others.clusterApplication(ca);
        }

        public void ClusterUser(Delta<Relationship<Cluster, User>> cu)
        {
            Clients.Others.clusterUser(cu);
        }
    }
}
