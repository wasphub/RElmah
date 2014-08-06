namespace RElmah.Server.Domain
{
    public class Visibility
    {
        public Visibility(string cluster, string user)
        {
            Cluster = cluster;
            User = user;
        }

        public string Cluster { get; private set; }
        public string User { get; private set; }
    }
}