namespace RElmah.Server.Domain
{
    public class ClusterUser
    {
        public ClusterUser(Cluster cluster, string user)
        {
            Cluster = cluster;
            User = user;
        }

        public Cluster Cluster { get; private set; }
        public string User { get; private set; }
    }
}