using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Common.Extensions;
using RElmah.Common.Model;
using RElmah.Foundation;
using RElmah.Publishers;
using RElmah.Services.Nulls;
using RElmah.Visibility;

namespace RElmah.Services
{
    public class VisibilityHolder : IVisibilityPublisher, IVisibilityPersistor
    {
        delegate ImmutableHashSet<T> HashsetJunction<T>(ImmutableHashSet<T> set, IEnumerable<T> sources);

        private readonly IVisibilityStore _visibilityStore;

        private readonly Subject<Delta<Cluster>>                       _clusterDeltas           = new Subject<Delta<Cluster>>();
        private readonly Subject<Delta<Source>>                        _sourceDeltas            = new Subject<Delta<Source>>();
        private readonly Subject<Delta<User>>                          _userDeltas              = new Subject<Delta<User>>();
        private readonly Subject<Delta<Relationship<Cluster, User>>>   _clusterUserOperations   = new Subject<Delta<Relationship<Cluster, User>>>();
        private readonly Subject<Delta<Relationship<Cluster, Source>>> _clusterSourceOperations = new Subject<Delta<Relationship<Cluster, Source>>>();

        private readonly IObservable<Delta<Cluster>>                       _clusterDeltasStream;
        private readonly IObservable<Delta<Source>>                        _sourceDeltasStream;
        private readonly IObservable<Delta<User>>                          _userDeltasStream;
        private readonly IObservable<Delta<Relationship<Cluster, User>>>   _clusterUserOperationsStream;
        private readonly IObservable<Delta<Relationship<Cluster, Source>>> _clusterSourceOperationsStream;

        private readonly AtomicImmutableDictionary<string, ImmutableHashSet<Source>> _usersSources   = new AtomicImmutableDictionary<string, ImmutableHashSet<Source>>();
 
        public VisibilityHolder(IVisibilityStore visibilityStore)
        {
            _visibilityStore = visibilityStore;

            _clusterDeltasStream           = _clusterDeltas.Publish().RefCount();
            _sourceDeltasStream            = _sourceDeltas.Publish().RefCount();
            _userDeltasStream              = _userDeltas.Publish().RefCount();
            _clusterUserOperationsStream   = _clusterUserOperations.Publish().RefCount();
            _clusterSourceOperationsStream = _clusterSourceOperations.Publish().RefCount();

            HashsetJunction<Source> except = (c, sources) => c.Except(sources);
            HashsetJunction<Source> union  = (c, sources) => c.Union(sources);

            var clusterUsers =
                from p in _clusterUserOperations
                let op = p.Type == DeltaType.Added ? union : except
                let user = p.Target.Secondary.Name
                select new
                {
                    User    = user, 
                    Current = _usersSources.Get(user, ImmutableHashSet<Source>.Empty),
                    Sources = p.Target.Primary.Sources,
                    Op = op
                };
            clusterUsers.Subscribe(p => _usersSources.SetItem(p.User, p.Op(p.Current, p.Sources)));

            var clusterSources =
                from p in _clusterSourceOperations
                let op = p.Type == DeltaType.Added ? union : except
                from un in p.Target.Primary.Users
                let user = un.Name
                select new
                {
                    User = user,
                    Current = _usersSources.Get(user, ImmutableHashSet<Source>.Empty),
                    Sources = p.Target.Secondary.ToSingleton(),
                    Op = op
                };
            clusterSources.Subscribe(p => _usersSources.SetItem(p.User, p.Op(p.Current, p.Sources)));
        }

        public VisibilityHolder() : this(NullVisibilityStore.Instance) { }

        public IObservable<Delta<Cluster>> GetClustersSequence()
        {
            return _clusterDeltasStream;
        }

        public async Task<ValueOrError<Cluster>> AddCluster(string name, bool fromBackend = false)
        {
            var s = await _visibilityStore.AddCluster(name, fromBackend);

            if (s.HasValue) _clusterDeltas.OnNext(Delta.Create(s.Value, DeltaType.Added, fromBackend));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveCluster(string name, bool fromBackend = false)
        {
            var s = await _visibilityStore.RemoveCluster(name, fromBackend);

            if (s.HasValue) _clusterDeltas.OnNext(Delta.Create(Cluster.Create(name), DeltaType.Removed, fromBackend));

            return s;
        }

        public Task<IEnumerable<Cluster>> GetClusters()
        {
            return _visibilityStore.GetClusters();
        }

        public Task<ValueOrError<Cluster>> GetCluster(string name)
        {
            return _visibilityStore.GetCluster(name);
        }

        public IObservable<Delta<Source>> GetSourcesSequence()
        {
            return _sourceDeltasStream;
        }

        public async Task<ValueOrError<Source>> AddSource(string name, string description, bool fromBackend = false)
        {
            var s = await _visibilityStore.AddSource(name, description, fromBackend);

            if (s.HasValue) _sourceDeltas.OnNext(Delta.Create(s.Value, DeltaType.Added, fromBackend));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveSource(string name, bool fromBackend = false)
        {
            var s = await _visibilityStore.RemoveSource(name, fromBackend);

            if (s.HasValue) _sourceDeltas.OnNext(Delta.Create(Source.Create(name, string.Empty), DeltaType.Removed, fromBackend));

            return s;
        }

        public Task<IEnumerable<Source>> GetSources()
        {
            return _visibilityStore.GetSources();
        }

        public Task<ValueOrError<Source>> GetSource(string name)
        {
            return _visibilityStore.GetSource(name);
        }

        public IObservable<Delta<User>> GetUsersSequence()
        {
            return _userDeltasStream;
        }

        public async Task<ValueOrError<User>> AddUser(string name, bool fromBackend = false)
        {
            var s = await _visibilityStore.AddUser(name, fromBackend);

            if (s.HasValue) _userDeltas.OnNext(Delta.Create(s.Value, DeltaType.Added, fromBackend));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveUser(string name, bool fromBackend = false)
        {
            var s = await _visibilityStore.RemoveUser(name, fromBackend);

            if (s.HasValue) _userDeltas.OnNext(Delta.Create(User.Create(name), DeltaType.Removed, fromBackend));

            return s;
        }

        public Task<IEnumerable<User>> GetUsers()
        {
            return _visibilityStore.GetUsers();
        }

        public Task<ValueOrError<User>> GetUser(string name)
        {
            return _visibilityStore.GetUser(name);
        }

        public async Task<ValueOrError<Relationship<Cluster, User>>> AddUserToCluster(string cluster, string user, bool fromBackend = false)
        {
            var s = await _visibilityStore.AddUserToCluster(cluster, user, fromBackend);

            if (s.HasValue) _clusterUserOperations.OnNext(Delta.Create(Relationship.Create(s.Value.Primary, s.Value.Secondary), DeltaType.Added, fromBackend));

            return s;
        }

        public async Task<ValueOrError<Relationship<Cluster, User>>> RemoveUserFromCluster(string cluster, string user, bool fromBackend = false)
        {
            var s = await _visibilityStore.RemoveUserFromCluster(cluster, user, fromBackend);

            if (s.HasValue) _clusterUserOperations.OnNext(Delta.Create(Relationship.Create(s.Value.Primary, s.Value.Secondary), DeltaType.Removed, fromBackend));

            return s;
        }

        public async Task<ValueOrError<Relationship<Cluster, Source>>> AddSourceToCluster(string cluster, string source, bool fromBackend = false)
        {
            var s = await _visibilityStore.AddSourceToCluster(cluster, source, fromBackend);

            if (s.HasValue) _clusterSourceOperations.OnNext(Delta.Create(Relationship.Create(s.Value.Primary, s.Value.Secondary), DeltaType.Added, fromBackend));

            return s;
        }

        public async Task<ValueOrError<Relationship<Cluster, Source>>> RemoveSourceFromCluster(string cluster, string source, bool fromBackend = false)
        {
            var s = await _visibilityStore.RemoveSourceFromCluster(cluster, source, fromBackend);

            if (s.HasValue) _clusterSourceOperations.OnNext(Delta.Create(Relationship.Create(s.Value.Primary, s.Value.Secondary), DeltaType.Removed, fromBackend));

            return s;
        }

        public IObservable<Delta<Relationship<Cluster, User>>> GetClusterUsersSequence()
        {
            return _clusterUserOperationsStream;
        }

        public IObservable<Delta<Relationship<Cluster, Source>>> GetClusterSourcesSequence()
        {
            return _clusterSourceOperationsStream;
        }

        public Task<IEnumerable<Source>> GetUserSources(string user)
        {
            return Task.FromResult(
                _usersSources.ContainsKey(user)
                ? _usersSources[user]
                : Enumerable.Empty<Source>());
        }

        public async Task<ValueOrError<User>> AddUserToken(string user, string token)
        {
            return await _visibilityStore.AddUserToken(user, token);
        }

        public async Task<ValueOrError<User>> RemoveUserToken(string token)
        {
            return await _visibilityStore.RemoveUserToken(token);
        }
    }
}
