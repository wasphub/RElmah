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

        void UserApplications(string user, IEnumerable<string> added, IEnumerable<string> removed);

        void AddGroup(string token, string group);

        void RemoveGroup(string token, string group);
    }

    public interface IBackendNotifier
    {
        void Error(ErrorPayload payload);
        void Cluster(Delta<Cluster> payload);
        void Application(Delta<Application> payload);
        void User(Delta<User> payload);
        void ClusterUser(Delta<Relationship<Cluster, User>> payload);
        void ClusterApplication(Delta<Relationship<Cluster, Application>> payload);
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
        IObservable<Delta<Application>> GetApplicationsSequence();
        IObservable<Delta<User>> GetUsersSequence();
        IObservable<Delta<Relationship<Cluster, User>>> GetClusterUsersSequence();
        IObservable<Delta<Relationship<Cluster, Application>>> GetClusterApplicationsSequence();
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
        Task<ValueOrError<Recap>> GetApplicationsRecap(IEnumerable<Application> apps, Func<IEnumerable<ErrorPayload>, int> reducer);
    }
}

namespace RElmah.Domain
{
    public interface IDomainReader
    {
        Task<IEnumerable<Cluster>> GetClusters();
        Task<ValueOrError<Cluster>> GetCluster(string name);
        Task<IEnumerable<Application>> GetApplications();
        Task<ValueOrError<Application>> GetApplication(string name);
        Task<IEnumerable<User>> GetUsers();
        Task<ValueOrError<User>> GetUser(string name);
        Task<IEnumerable<Application>> GetUserApplications(string user);
    }

    public interface IDomainPersistor : IDomainReader
    {
        Task<ValueOrError<Cluster>> AddCluster(string name);
        Task<ValueOrError<bool>> RemoveCluster(string name);
        Task<ValueOrError<Application>> AddApplication(string name);
        Task<ValueOrError<bool>> RemoveApplication(string name);
        Task<ValueOrError<User>> AddUser(string name);
        Task<ValueOrError<bool>> RemoveUser(string name);
        Task<ValueOrError<Relationship<Cluster, User>>> AddUserToCluster(string cluster, string user);
        Task<ValueOrError<Relationship<Cluster, User>>> RemoveUserFromCluster(string cluster, string user);
        Task<ValueOrError<Relationship<Cluster, Application>>> AddApplicationToCluster(string cluster, string application);
        Task<ValueOrError<Relationship<Cluster, Application>>> RemoveApplicationFromCluster(string cluster, string application);
        Task<ValueOrError<User>> AddUserToken(string user, string token);
        Task<ValueOrError<User>> RemoveUserToken(string token);
    }

    public interface IDomainStore : IDomainPersistor
    {
    }
}

