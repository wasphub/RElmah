using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RElmah.Common;
using RElmah.Foundation;

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

        public void Source(Delta<Source> source)
        {
            Clients.Others.source(source);
        }

        public void User(Delta<User> user)
        {
            Clients.Others.user(user);
        }

        public void ClusterSource(Delta<Relationship<Cluster, Source>> cs)
        {
            Clients.Others.clusterSource(cs);
        }

        public void ClusterUser(Delta<Relationship<Cluster, User>> cu)
        {
            Clients.Others.clusterUser(cu);
        }
    }
}
