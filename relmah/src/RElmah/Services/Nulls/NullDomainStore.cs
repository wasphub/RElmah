using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Foundation;
using RElmah.Visibility;

namespace RElmah.Services.Nulls
{
    class NullVisibilityStore : IVisibilityStore
    {
        private NullVisibilityStore() { }

        public static IVisibilityStore Instance = new NullVisibilityStore();

        public Task<ValueOrError<Cluster>> AddCluster(string name, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<Cluster>(Cluster.Create(name)));
        }

        public Task<ValueOrError<bool>> RemoveCluster(string name, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<bool>(true));
        }

        public Task<IEnumerable<Cluster>> GetClusters()
        {
            return Task.FromResult(Enumerable.Empty<Cluster>());
        }

        public Task<ValueOrError<Cluster>> GetCluster(string name)
        {
            return Task.FromResult(new ValueOrError<Cluster>(Cluster.Create(name)));
        }

        public Task<ValueOrError<Source>> AddSource(string name, string description, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<Source>(Source.Create(name, description)));
        }

        public Task<ValueOrError<bool>> RemoveSource(string name, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<bool>(true));
        }

        public Task<IEnumerable<Source>> GetSources()
        {
            return Task.FromResult(Enumerable.Empty<Source>());
        }

        public Task<ValueOrError<Source>> GetSource(string name)
        {
            return Task.FromResult(new ValueOrError<Source>(Source.Create(name, string.Empty)));
        }

        public Task<ValueOrError<User>> AddUser(string name, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<User>(User.Create(name)));
        }

        public Task<ValueOrError<bool>> RemoveUser(string name, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<bool>(true));
        }

        public Task<IEnumerable<User>> GetUsers()
        {
            return Task.FromResult(Enumerable.Empty<User>());
        }

        public Task<ValueOrError<User>> GetUser(string name)
        {
            return Task.FromResult(new ValueOrError<User>(User.Create(name)));
        }

        public Task<ValueOrError<Relationship<Cluster, User>>> AddUserToCluster(string cluster, string user, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<Relationship<Cluster, User>>(new Relationship<Cluster, User>(Cluster.Create(cluster), User.Create(user))));
        }

        public Task<ValueOrError<Relationship<Cluster, User>>> RemoveUserFromCluster(string cluster, string user, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<Relationship<Cluster, User>>(new Relationship<Cluster, User>(Cluster.Create(cluster), User.Create(user))));
        }

        public Task<ValueOrError<Relationship<Cluster, Source>>> AddSourceToCluster(string cluster, string source, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<Relationship<Cluster, Source>>(new Relationship<Cluster, Source>(Cluster.Create(cluster), Source.Create(source, string.Empty))));
        }

        public Task<ValueOrError<Relationship<Cluster, Source>>> RemoveSourceFromCluster(string cluster, string source, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<Relationship<Cluster, Source>>(new Relationship<Cluster, Source>(Cluster.Create(cluster), Source.Create(source, string.Empty))));
        }

        public Task<IEnumerable<Source>> GetUserSources(string user)
        {
            return Task.FromResult(Enumerable.Empty<Source>());
        }

        public Task<ValueOrError<User>> AddUserToken(string user, string token)
        {
            return Task.FromResult(new ValueOrError<User>(User.Create(user).AddToken(token)));
        }

        public Task<ValueOrError<User>> RemoveUserToken(string token)
        {
            return Task.FromResult(new ValueOrError<User>((User)null));
        }

        public Task<ValueOrError<Recap>> GetSourcesRecap(IEnumerable<Source> sources)
        {
            return Task.FromResult(new ValueOrError<Recap>((Recap)null));
        }
    }
}
