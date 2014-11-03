using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;

namespace RElmah.Services
{
    class InMemoryConfigurationStore : IConfigurationUpdater
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
    }
}
