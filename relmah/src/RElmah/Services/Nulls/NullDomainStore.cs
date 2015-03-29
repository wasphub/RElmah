using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Domain;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Services.Nulls
{
    class NullDomainStore : IDomainStore
    {
        private NullDomainStore() { }

        public static IDomainStore Instance = new NullDomainStore();

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

        public Task<ValueOrError<Application>> AddApplication(string name, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<Application>(Application.Create(name)));
        }

        public Task<ValueOrError<bool>> RemoveApplication(string name, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<bool>(true));
        }

        public Task<IEnumerable<Application>> GetApplications()
        {
            return Task.FromResult(Enumerable.Empty<Application>());
        }

        public Task<ValueOrError<Application>> GetApplication(string name)
        {
            return Task.FromResult(new ValueOrError<Application>(Application.Create(name)));
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

        public Task<ValueOrError<Relationship<Cluster, Application>>> AddApplicationToCluster(string cluster, string application, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<Relationship<Cluster, Application>>(new Relationship<Cluster, Application>(Cluster.Create(cluster), Application.Create(application))));
        }

        public Task<ValueOrError<Relationship<Cluster, Application>>> RemoveApplicationFromCluster(string cluster, string application, bool fromBackend = false)
        {
            return Task.FromResult(new ValueOrError<Relationship<Cluster, Application>>(new Relationship<Cluster, Application>(Cluster.Create(cluster), Application.Create(application))));
        }

        public Task<IEnumerable<Application>> GetUserApplications(string user)
        {
            return Task.FromResult(Enumerable.Empty<Application>());
        }

        public Task<ValueOrError<User>> AddUserToken(string user, string token)
        {
            return Task.FromResult(new ValueOrError<User>(User.Create(user).AddToken(token)));
        }

        public Task<ValueOrError<User>> RemoveUserToken(string token)
        {
            return Task.FromResult(new ValueOrError<User>((User)null));
        }

        public Task<ValueOrError<Recap>> GetApplicationsRecap(IEnumerable<Application> apps)
        {
            return Task.FromResult(new ValueOrError<Recap>((Recap)null));
        }
    }
}
