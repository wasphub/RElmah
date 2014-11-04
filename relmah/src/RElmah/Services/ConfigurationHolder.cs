﻿using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;
using RElmah.Services.Nulls;

namespace RElmah.Services
{
    public class ConfigurationHolder : IConfigurationProvider, IConfigurationUpdater
    {
        private readonly IConfigurationStore _configurationStore;

        private readonly Subject<Delta<Cluster>>                            _clusterDeltas                = new Subject<Delta<Cluster>>();
        private readonly Subject<Delta<Application>>                        _applicationDeltas            = new Subject<Delta<Application>>();
        private readonly Subject<Delta<User>>                               _userDeltas                   = new Subject<Delta<User>>();

        private readonly Subject<Delta<Relationship<Cluster, User>>>        _clusterUserOperations        = new Subject<Delta<Relationship<Cluster, User>>>();
        private readonly Subject<Delta<Relationship<Cluster, Application>>> _clusterApplicationOperations = new Subject<Delta<Relationship<Cluster, Application>>>();

        public ConfigurationHolder(IConfigurationStore configurationStore)
        {
            _configurationStore = configurationStore;
        }

        public ConfigurationHolder() : this(new NullConfigurationStore()) { }

        public IObservable<Delta<Cluster>> ObserveClusters()
        {
            return _clusterDeltas;
        }

        public async Task<ValueOrError<Cluster>> AddCluster(string name)
        {
            var s = await _configurationStore.AddCluster(name);

            if (s.HasValue) _clusterDeltas.OnNext(Delta.Create(s.Value, DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveCluster(string name)
        {
            var s = await _configurationStore.RemoveCluster(name);

            if (s.HasValue) _clusterDeltas.OnNext(Delta.Create(Cluster.Create(name), DeltaType.Removed));

            return s;
        }

        public Task<IEnumerable<Cluster>> GetClusters()
        {
            return _configurationStore.GetClusters();
        }

        public Task<ValueOrError<Cluster>> GetCluster(string name)
        {
            return _configurationStore.GetCluster(name);
        }

        public IObservable<Delta<Application>> ObserveApplications()
        {
            return _applicationDeltas;
        }

        public async Task<ValueOrError<Application>> AddApplication(string name)
        {
            var s = await _configurationStore.AddApplication(name);

            if (s.HasValue) _applicationDeltas.OnNext(Delta.Create(s.Value, DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveApplication(string name)
        {
            var s = await _configurationStore.RemoveApplication(name);

            if (s.HasValue) _applicationDeltas.OnNext(Delta.Create(Application.Create(name), DeltaType.Removed));

            return s;
        }

        public Task<IEnumerable<Application>> GetApplications()
        {
            return _configurationStore.GetApplications();
        }

        public Task<ValueOrError<Application>> GetApplication(string name)
        {
            return _configurationStore.GetApplication(name);
        }

        public IObservable<Delta<User>> ObserveUsers()
        {
            return _userDeltas;
        }

        public async Task<ValueOrError<User>> AddUser(string name)
        {
            var s = await _configurationStore.AddUser(name);

            if (s.HasValue) _userDeltas.OnNext(Delta.Create(s.Value, DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<bool>> RemoveUser(string name)
        {
            var s = await _configurationStore.RemoveUser(name);

            if (s.HasValue) _userDeltas.OnNext(Delta.Create(User.Create(name), DeltaType.Removed));

            return s;
        }

        public Task<IEnumerable<User>> GetUsers()
        {
            return _configurationStore.GetUsers();
        }

        public Task<ValueOrError<User>> GetUser(string name)
        {
            return _configurationStore.GetUser(name);
        }

        public async Task<ValueOrError<Relationship<Cluster, User>>> AddUserToCluster(string cluster, string user)
        {
            var s = await _configurationStore.AddUserToCluster(cluster, user);

            if (s.HasValue) _clusterUserOperations.OnNext(Delta.Create(Relationship.Create(s.Value.Primary, s.Value.Secondary), DeltaType.Added));

            return s;
        }

        public async Task<ValueOrError<Relationship<Cluster, Application>>> AddApplicationToCluster(string cluster, string application)
        {
            var s = await _configurationStore.AddApplicationToCluster(cluster, application);

            if (s.HasValue) _clusterApplicationOperations.OnNext(Delta.Create(Relationship.Create(s.Value.Primary, s.Value.Secondary), DeltaType.Added));

            return s;
        }

        public IObservable<Delta<Relationship<Cluster, User>>> ObserveClusterUsers()
        {
            return _clusterUserOperations;
        }

        public IObservable<Delta<Relationship<Cluster, Application>>> ObserveClusterApplications()
        {
            return _clusterApplicationOperations;
        }
    }
}
