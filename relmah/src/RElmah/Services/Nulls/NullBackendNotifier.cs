using RElmah.Common;
using RElmah.Foundation;
using RElmah.Notifiers;

namespace RElmah.Services.Nulls
{
    public class NullBackendNotifier : IBackendNotifier
    {
        private NullBackendNotifier() { }

        public static IBackendNotifier Instance = new NullBackendNotifier();
 
        public void Error(ErrorPayload payload)
        {
        }

        public void Cluster(Delta<Cluster> payload)
        {
        }

        public void Source(Delta<Source> payload)
        {
        }

        public void User(Delta<User> payload)
        {
        }

        public void ClusterUser(Delta<Relationship<Cluster, User>> payload)
        {
        }

        public void ClusterSource(Delta<Relationship<Cluster, Source>> payload)
        {
        }
    }
}
