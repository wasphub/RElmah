using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RElmah.Grounding;
using RElmah.Models;

namespace RElmah.Services
{
    public class InMemoryDomainStore : IDomainStore
    {
        private readonly AtomicImmutableDictionary<string, Cluster> _clusters =
            new AtomicImmutableDictionary<string, Cluster>();

        private readonly AtomicImmutableDictionary<string, User> _users =
            new AtomicImmutableDictionary<string, User>();

        private readonly AtomicImmutableDictionary<string, Application> _applications =
            new AtomicImmutableDictionary<string, Application>();

        public Task<ValueOrError<Cluster>> AddCluster(string name)
        {
            var cluster = Cluster.Create(name);
            _clusters.Add(cluster.Name, cluster);
            return Task.Factory.StartNew(() => ValueOrError.Create(cluster));
        }

        public Task<ValueOrError<bool>> RemoveCluster(string name)
        {
            _clusters.Remove(name);

            return Task.Factory.StartNew(() => ValueOrError.Create(true));
        }

        public Task<IEnumerable<Cluster>> GetClusters()
        {
            return Task.FromResult(_clusters.Values);
        }

        public Task<ValueOrError<Cluster>> GetCluster(string name)
        {
            return Task.FromResult(new ValueOrError<Cluster>(_clusters[name]));
        }

        public Task<ValueOrError<Application>> AddApplication(string name)
        {
            var application = Application.Create(name);
            _applications.Add(application.Name, application);
            return Task.Factory.StartNew(() => ValueOrError.Create(application));
        }

        public Task<ValueOrError<bool>> RemoveApplication(string name)
        {
            _applications.Remove(name);

            return Task.Factory.StartNew(() => ValueOrError.Create(true));
        }

        public Task<IEnumerable<Application>> GetApplications()
        {
            return Task.FromResult(_applications.Values);
        }

        public Task<ValueOrError<Application>> GetApplication(string name)
        {
            return Task.FromResult(new ValueOrError<Application>(_applications[name]));
        }

        public Task<ValueOrError<User>> AddUser(string name)
        {
            var user = User.Create(name);
            _users.Add(user.Name, user);
            return Task.Factory.StartNew(() => ValueOrError.Create(user));
        }

        public Task<ValueOrError<bool>> RemoveUser(string name)
        {
            _users.Remove(name);

            return Task.Factory.StartNew(() => ValueOrError.Create(true));
        }

        public Task<IEnumerable<User>> GetUsers()
        {
            return Task.FromResult(_users.Values);
        }

        public Task<ValueOrError<User>> GetUser(string name)
        {
            return Task.FromResult(new ValueOrError<User>(_users[name]));
        }

        public Task<ValueOrError<Relationship<Cluster, User>>> AddUserToCluster(string cluster, string user)
        {
            var c = _clusters[cluster];
            var u = _users[user];

            var value = c.SetUser(u);
            _clusters.SetItem(cluster, value);
            return Task.FromResult(ValueOrError.Create(Relationship.Create(value, u)));
        }

        public Task<ValueOrError<Relationship<Cluster, User>>> RemoveUserFromCluster(string cluster, string user)
        {
            var c = _clusters[cluster];
            var u = _users[user];

            var value = c.RemoveUser(u);
            _clusters.SetItem(cluster, value);
            return Task.FromResult(ValueOrError.Create(Relationship.Create(value, u)));
        }

        public Task<ValueOrError<Relationship<Cluster, Application>>> AddApplicationToCluster(string cluster, string application)
        {
            var c = _clusters[cluster];
            var a = _applications[application];

            var value = c.AddApplication(a);
            _clusters.SetItem(cluster, value);
            return Task.FromResult(ValueOrError.Create(Relationship.Create(value, a)));
        }

        public Task<ValueOrError<Relationship<Cluster, Application>>> RemoveApplicationFromCluster(string cluster, string application)
        {
            var c = _clusters[cluster];
            var a = _applications[application];

            var value = c.RemoveApplication(a);
            _clusters.SetItem(cluster, value);
            return Task.FromResult(ValueOrError.Create(Relationship.Create(value, a)));
        }

        public IEnumerable<Application> GetUserApplications(string user)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<User>> AddUserToken(string user, string token)
        {
            var u = _users[user].AddToken(token);
            _users.SetItem(user, u);

            foreach (var c in _clusters.Values.Where(c => c.HasUser(user)))
                _clusters.SetItem(c.Name, c.SetUser(u));
            return Task.FromResult(new ValueOrError<User>(u));
        }
    }
}
