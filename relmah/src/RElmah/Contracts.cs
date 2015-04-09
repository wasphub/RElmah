using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Domain;
using RElmah.Errors;
using RElmah.Foundation;
using RElmah.Models;
using RElmah.Notifiers;
using RElmah.Publishers;

namespace RElmah
{
    public interface IIdentityProvider
    {
        IIdentity GetIdentity(object request);
    }

    public interface IResolver
    {
        T Resolve<T>();
    }

    public interface IRegistry : IResolver
    {
        void Register<T>(Func<T> supplier);
        void Register(Type type, Func<object> supplier);
    }
}

namespace RElmah.Notifiers
{
    public interface IFrontendNotifier
    {
        void Recap(string user, Recap recap);

        void Error(string user, ErrorPayload payload);

        void UserSources(string user, IEnumerable<string> added, IEnumerable<string> removed);

        void AddGroup(string token, string group);

        void RemoveGroup(string token, string group);
    }

    public interface IBackendNotifier
    {
        void Error(ErrorPayload payload);
        void Cluster(Delta<Cluster> payload);
        void Source(Delta<Source> payload);
        void User(Delta<User> payload);
        void ClusterUser(Delta<Relationship<Cluster, User>> payload);
        void ClusterSource(Delta<Relationship<Cluster, Source>> payload);
    }
}

namespace RElmah.Queries
{
    public interface IFrontendQuery
    {
        Task<IDisposable> Run(ValueOrError<User> user, RunTargets targets);
    }

    public interface IFrontendQueriesFactory
    {
        void Setup(string user, string token, Action<string> connector);
        void Teardown(string token);
    }

    public interface IBackendQuery
    {
        Task<IDisposable> Run(RunTargets targets);
    }

    public interface IBackendQueriesFactory
    {
        void Setup();
    }

    public class RunTargets
    {
        public IFrontendNotifier FrontendNotifier { get; set; }
        public IBackendNotifier BackendNotifier { get; set; }
        public IErrorsInbox ErrorsInbox { get; set; }
        public IErrorsInbox BackendErrorsInbox { get; set; }
        public IErrorsBacklog ErrorsBacklog { get; set; }
        public IDomainPersistor DomainPersistor { get; set; }
        public IDomainPublisher DomainPublisher { get; set; }
    }
}

namespace RElmah.Publishers
{
    public interface IErrorsPublisher
    {
        IObservable<ErrorPayload> GetErrorsStream();
    }

    public interface IDomainPublisher
    {
        IObservable<Delta<Cluster>> GetClustersSequence();
        IObservable<Delta<Source>> GetSourcesSequence();
        IObservable<Delta<User>> GetUsersSequence();
        IObservable<Delta<Relationship<Cluster, User>>> GetClusterUsersSequence();
        IObservable<Delta<Relationship<Cluster, Source>>> GetClusterSourcesSequence();
    }
}

namespace RElmah.Errors
{
    public interface IErrorsInbox : IErrorsPublisher
    {
        Task Post(ErrorPayload payload);
    }

    public interface IErrorsBacklog
    {
        Task Store(ErrorPayload payload);
        Task<ValueOrError<Recap>> GetSourcesRecap(IEnumerable<Source> sources, Func<IEnumerable<ErrorPayload>, int> reducer);
    }
}

namespace RElmah.Domain
{
    public interface IDomainReader
    {
        Task<IEnumerable<Cluster>> GetClusters();
        Task<ValueOrError<Cluster>> GetCluster(string name);
        Task<IEnumerable<Source>> GetSources();
        Task<ValueOrError<Source>> GetSource(string name);
        Task<IEnumerable<User>> GetUsers();
        Task<ValueOrError<User>> GetUser(string name);
        Task<IEnumerable<Source>> GetUserSources(string user);
    }

    public interface IDomainPersistor : IDomainReader
    {
        Task<ValueOrError<Cluster>> AddCluster(string name, bool fromBackend = false);
        Task<ValueOrError<bool>> RemoveCluster(string name, bool fromBackend = false);
        Task<ValueOrError<Source>> AddSource(string name, bool fromBackend = false);
        Task<ValueOrError<bool>> RemoveSource(string name, bool fromBackend = false);
        Task<ValueOrError<User>> AddUser(string name, bool fromBackend = false);
        Task<ValueOrError<bool>> RemoveUser(string name, bool fromBackend = false);
        Task<ValueOrError<Relationship<Cluster, User>>> AddUserToCluster(string cluster, string user, bool fromBackend = false);
        Task<ValueOrError<Relationship<Cluster, User>>> RemoveUserFromCluster(string cluster, string user, bool fromBackend = false);
        Task<ValueOrError<Relationship<Cluster, Source>>> AddSourceToCluster(string cluster, string source, bool fromBackend = false);
        Task<ValueOrError<Relationship<Cluster, Source>>> RemoveSourceFromCluster(string cluster, string source, bool fromBackend = false);
        Task<ValueOrError<User>> AddUserToken(string user, string token);
        Task<ValueOrError<User>> RemoveUserToken(string token);
    }

    public interface IDomainStore : IDomainPersistor
    {
    }
}

