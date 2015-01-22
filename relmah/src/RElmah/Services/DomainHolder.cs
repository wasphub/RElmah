using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Extensions;
using RElmah.Foundation;
using RElmah.Models;
using RElmah.Services.Nulls;

namespace RElmah.Services
{
    public class DomainHolder : IDomainPublisher, IDomainPersistor
    {
        delegate ImmutableHashSet<T> HashsetJunction<T>(ImmutableHashSet<T> set, IEnumerable<T> apps);

        private readonly IDomainStore _domainStore;

        private readonly Subject<Delta<Cluster>>                            _clusterDeltas                = new Subject<Delta<Cluster>>();
        private readonly Subject<Delta<Application>>                        _applicationDeltas            = new Subject<Delta<Application>>();
        private readonly Subject<Delta<User>>                               _userDeltas                   = new Subject<Delta<User>>();

        private readonly Subject<Delta<Relationship<Cluster, User>>>        _clusterUserOperations        = new Subject<Delta<Relationship<Cluster, User>>>();
        private readonly Subject<Delta<Relationship<Cluster, Application>>> _clusterApplicationOperations = new Subject<Delta<Relationship<Cluster, Application>>>();

        private readonly AtomicImmutableDictionary<string, ImmutableHashSet<Application>> _usersApplications   = new AtomicImmutableDictionary<string, ImmutableHashSet<Application>>();
 
        public DomainHolder(IDomainStore domainStore)
        {
            _domainStore = domainStore;

            HashsetJunction<Application> except = (c, apps) => c.Except(apps);
            HashsetJunction<Application> union  = (c, apps) => c.Union(apps);

            var clusterUsers =
                from p in _clusterUserOperations
                let op = p.Type == DeltaType.Added ? union : except
                let user = p.Target.Secondary.Name
                select new
                {
                    User = user, 
                    Current = _usersApplications.Get(user, ImmutableHashSet<Application>.Empty),
                    p.Target.Primary.Applications,
                    Op = op
                };
            clusterUsers.Subscribe(p => _usersApplications.SetItem(p.User, p.Op(p.Current, p.Applications)));

            var clusterApps =
                from p in _clusterApplicationOperations
                let op = p.Type == DeltaType.Added ? union : except
                from un in p.Target.Primary.Users
                let user = un.Name
                select new
                {
                    User = user,
                    Current = _usersApplications.Get(user, ImmutableHashSet<Application>.Empty),
                    Applications = p.Target.Secondary.ToSingleton(),
                    Op = op
                };
            clusterApps.Subscribe(p => _usersApplications.SetItem(p.User, p.Op(p.Current, p.Applications)));
        }

        public DomainHolder() : this(new NullDomainStore()) { }

        public IObservable<Delta<Cluster>> GetClustersSequence()
        {
            return _clusterDeltas;
        }

        public async Task<ValueOrError<Cluster>> AddCluster(string name)
        {
            var s = await _domainStore.AddCluster(name);

            if (s.HasValue) _clusterDeltas.OnNext(Delta.Create(s.Value, DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveCluster(string name)
        {
            var s = await _domainStore.RemoveCluster(name);

            if (s.HasValue) _clusterDeltas.OnNext(Delta.Create(Cluster.Create(name), DeltaType.Removed));

            return s;
        }

        public Task<IEnumerable<Cluster>> GetClusters()
        {
            return _domainStore.GetClusters();
        }

        public Task<ValueOrError<Cluster>> GetCluster(string name)
        {
            return _domainStore.GetCluster(name);
        }

        public IObservable<Delta<Application>> GetApplicationsSequence()
        {
            return _applicationDeltas;
        }

        public async Task<ValueOrError<Application>> AddApplication(string name)
        {
            var s = await _domainStore.AddApplication(name);

            if (s.HasValue) _applicationDeltas.OnNext(Delta.Create(s.Value, DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveApplication(string name)
        {
            var s = await _domainStore.RemoveApplication(name);

            if (s.HasValue) _applicationDeltas.OnNext(Delta.Create(Application.Create(name), DeltaType.Removed));

            return s;
        }

        public Task<IEnumerable<Application>> GetApplications()
        {
            return _domainStore.GetApplications();
        }

        public Task<ValueOrError<Application>> GetApplication(string name)
        {
            return _domainStore.GetApplication(name);
        }

        public IObservable<Delta<User>> GetUsersSequence()
        {
            return _userDeltas;
        }

        public async Task<ValueOrError<User>> AddUser(string name)
        {
            var s = await _domainStore.AddUser(name);

            if (s.HasValue) _userDeltas.OnNext(Delta.Create(s.Value, DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveUser(string name)
        {
            var s = await _domainStore.RemoveUser(name);

            if (s.HasValue) _userDeltas.OnNext(Delta.Create(User.Create(name), DeltaType.Removed));

            return s;
        }

        public Task<IEnumerable<User>> GetUsers()
        {
            return _domainStore.GetUsers();
        }

        public Task<ValueOrError<User>> GetUser(string name)
        {
            return _domainStore.GetUser(name);
        }

        public async Task<ValueOrError<Relationship<Cluster, User>>> AddUserToCluster(string cluster, string user)
        {
            var s = await _domainStore.AddUserToCluster(cluster, user);

            if (s.HasValue) _clusterUserOperations.OnNext(Delta.Create(Relationship.Create(s.Value.Primary, s.Value.Secondary), DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<Relationship<Cluster, User>>> RemoveUserFromCluster(string cluster, string user)
        {
            var s = await _domainStore.RemoveUserFromCluster(cluster, user);

            if (s.HasValue) _clusterUserOperations.OnNext(Delta.Create(Relationship.Create(s.Value.Primary, s.Value.Secondary), DeltaType.Removed));

            return s;
        }

        public async Task<ValueOrError<Relationship<Cluster, Application>>> AddApplicationToCluster(string cluster, string application)
        {
            var s = await _domainStore.AddApplicationToCluster(cluster, application);

            if (s.HasValue) _clusterApplicationOperations.OnNext(Delta.Create(Relationship.Create(s.Value.Primary, s.Value.Secondary), DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<Relationship<Cluster, Application>>> RemoveApplicationFromCluster(string cluster, string application)
        {
            var s = await _domainStore.RemoveApplicationFromCluster(cluster, application);

            if (s.HasValue) _clusterApplicationOperations.OnNext(Delta.Create(Relationship.Create(s.Value.Primary, s.Value.Secondary), DeltaType.Removed));

            return s;
        }

        public IObservable<Delta<Relationship<Cluster, User>>> GetClusterUsersSequence()
        {
            return _clusterUserOperations;
        }

        public IObservable<Delta<Relationship<Cluster, Application>>> GetClusterApplicationsSequence()
        {
            return _clusterApplicationOperations;
        }

        public Task<IEnumerable<Application>> GetUserApplications(string user)
        {
            return Task.FromResult(
                _usersApplications.ContainsKey(user)
                ? _usersApplications[user]
                : Enumerable.Empty<Application>());
        }

        public async Task<ValueOrError<User>> AddUserToken(string user, string token)
        {
            return await _domainStore.AddUserToken(user, token);
        }

        public async Task<ValueOrError<User>> RemoveUserToken(string token)
        {
            return await _domainStore.RemoveUserToken(token);
        }
    }
}
