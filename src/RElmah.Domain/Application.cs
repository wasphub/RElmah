namespace RElmah.Domain
{
    public class Application
    {
        public Application(string name, string sourceId, Cluster cluster)
        {
            Name = name;
            SourceId = sourceId;
            Cluster = cluster;
        }

        public string Name { get; private set; }
        public string SourceId { get; private set; }

        public Cluster Cluster { get; private set; }
    }
}