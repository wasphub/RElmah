using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using RElmah.Common.Model;
using RElmah.Errors;
using RElmah.Foundation;
using RElmah.Notifiers;
using RElmah.Publishers;
using RElmah.Visibility;

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

        void UserSources(string user, IEnumerable<Source> added, IEnumerable<Source> removed);
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
        public IErrorsBacklogReader ErrorsBacklogReader { get; set; }
        public IVisibilityPersistor VisibilityPersistor { get; set; }
        public IVisibilityPublisher VisibilityPublisher { get; set; }
    }
}

namespace RElmah.Publishers
{
    public interface IErrorsPublisher
    {
        IObservable<ErrorPayload> GetErrorsStream();
        IObservable<IList<ErrorPayload>> GetErrorBuffersStream();
    }

    public interface IVisibilityPublisher
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
    }

    public interface IErrorsBacklogReader
    {
        Task<ValueOrError<Recap>> GetSourcesRecap(IEnumerable<Source> sources);
    }
}

namespace RElmah.Visibility
{
    public interface IVisibilityAccessor
    {
        Task<IEnumerable<Cluster>> GetClusters();
        Task<ValueOrError<Cluster>> GetCluster(string name);
        Task<IEnumerable<Source>> GetSources();
        Task<ValueOrError<Source>> GetSource(string name);
        Task<IEnumerable<User>> GetUsers();
        Task<ValueOrError<User>> GetUser(string name);
        Task<IEnumerable<Source>> GetUserSources(string user);
    }

    public interface IVisibilityPersistor : IVisibilityAccessor
    {
        Task<ValueOrError<Cluster>> AddCluster(string name, bool fromBackend = false);
        Task<ValueOrError<bool>> RemoveCluster(string name, bool fromBackend = false);
        Task<ValueOrError<Source>> AddSource(string name, string description, bool fromBackend = false);
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

    public interface IVisibilityStore : IVisibilityPersistor
    {
    }
}

