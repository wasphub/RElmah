using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RElmah.Domain;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Services
{
    public class InMemoryDomainStore : IDomainStore
    {
        private readonly AtomicImmutableDictionary<string, Cluster> _clusters =
            new AtomicImmutableDictionary<string, Cluster>();

        private readonly AtomicImmutableDictionary<string, User> _users =
            new AtomicImmutableDictionary<string, User>();

        private readonly AtomicImmutableDictionary<string, Source> _sources =
            new AtomicImmutableDictionary<string, Source>();

        public Task<ValueOrError<Cluster>> AddCluster(string name, bool fromBackend = false)
        {
            var cluster = Cluster.Create(name);
            _clusters.Add(cluster.Name, cluster);
            return Task.FromResult(ValueOrError.Create(cluster));
        }

        public Task<ValueOrError<bool>> RemoveCluster(string name, bool fromBackend = false)
        {
            _clusters.Remove(name);

            return Task.FromResult(ValueOrError.Create(true));
        }

        public Task<IEnumerable<Cluster>> GetClusters()
        {
            return Task.FromResult(_clusters.Values);
        }

        public Task<ValueOrError<Cluster>> GetCluster(string name)
        {
            return Task.FromResult(new ValueOrError<Cluster>(_clusters[name]));
        }

        public Task<ValueOrError<Source>> AddSource(string name, bool fromBackend = false)
        {
            var source = Source.Create(name);
            _sources.Add(source.SourceId, source);

            return Task.FromResult(ValueOrError.Create(source));
        }

        public Task<ValueOrError<bool>> RemoveSource(string name, bool fromBackend = false)
        {
            _sources.Remove(name);

            return Task.FromResult(ValueOrError.Create(true));
        }

        public Task<IEnumerable<Source>> GetSources()
        {
            return Task.FromResult(_sources.Values);
        }

        public Task<ValueOrError<Source>> GetSource(string name)
        {
            return Task.FromResult(new ValueOrError<Source>(_sources[name]));
        }

        public Task<ValueOrError<User>> AddUser(string name, bool fromBackend = false)
        {
            var user = User.Create(name);
            _users.Add(user.Name, user);
            return Task.FromResult(ValueOrError.Create(user));
        }

        public Task<ValueOrError<bool>> RemoveUser(string name, bool fromBackend = false)
        {
            _users.Remove(name);

            return Task.FromResult(ValueOrError.Create(true));
        }

        public Task<IEnumerable<User>> GetUsers()
        {
            return Task.FromResult(_users.Values);
        }

        public Task<ValueOrError<User>> GetUser(string name)
        {
            return Task.FromResult(new ValueOrError<User>(_users[name]));
        }

        public Task<ValueOrError<Relationship<Cluster, User>>> AddUserToCluster(string cluster, string user, bool fromBackend = false)
        {
            var c = _clusters[cluster];
            var u = _users[user];

            var value = c.SetUser(u);
            _clusters.SetItem(cluster, value);
            return Task.FromResult(ValueOrError.Create(Relationship.Create(value, u)));
        }

        public Task<ValueOrError<Relationship<Cluster, User>>> RemoveUserFromCluster(string cluster, string user, bool fromBackend = false)
        {
            var c = _clusters[cluster];
            var u = _users[user];

            var value = c.RemoveUser(u);
            _clusters.SetItem(cluster, value);
            return Task.FromResult(ValueOrError.Create(Relationship.Create(value, u)));
        }

        public Task<ValueOrError<Relationship<Cluster, Source>>> AddSourceToCluster(string cluster, string source, bool fromBackend = false)
        {
            var c = _clusters[cluster];
            var a = _sources[source];

            var value = c.AddSource(a);
            _clusters.SetItem(cluster, value);
            return Task.FromResult(ValueOrError.Create(Relationship.Create(value, a)));
        }

        public Task<ValueOrError<Relationship<Cluster, Source>>> RemoveSourceFromCluster(string cluster, string source, bool fromBackend = false)
        {
            var c = _clusters[cluster];
            var a = _sources[source];

            var value = c.RemoveSource(a);
            _clusters.SetItem(cluster, value);
            return Task.FromResult(ValueOrError.Create(Relationship.Create(value, a)));
        }

        public Task<IEnumerable<Source>> GetUserSources(string user)
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

        public Task<ValueOrError<User>> RemoveUserToken(string token)
        {
            var us =
                from u in _users.Values
                where u.Tokens.Any(t => t == token)
                select u;

            var voe = us.SingleOrDefault().ToValueOrError();
            if (!voe.HasValue) return Task.FromResult(ValueOrError.Null<User>());

            var user = voe.Value;

            user = user.RemoveToken(token);
            _users.SetItem(user.Name, user);

            foreach (var c in _clusters.Values.Where(c => c.HasUser(user.Name)))
                _clusters.SetItem(c.Name, c.SetUser(user));

            return Task.FromResult(new ValueOrError<User>(user));
        }
    }
}
