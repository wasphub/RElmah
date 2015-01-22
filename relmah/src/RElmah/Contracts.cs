using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah
{
    /// <summary>
    /// This contract receives errors from the 
    /// outside world, 'parks' them and makes
    /// them observable. The 'parking' strategy
    /// is left to the implementor.
    /// </summary>
    public interface IErrorsInbox
    {
        Task Post(ErrorPayload payload);
        IObservable<ErrorPayload> GetErrorsStream();
        Task<ValueOrError<Recap>> GetApplicationsRecap(IEnumerable<Application> apps);
    }

    /// <summary>
    /// This contract represents a persistent
    /// store where errors are saved. Storing an
    /// error could be a lenghty operation,
    /// therefore the action returns a Task.
    /// </summary>
    public interface IErrorsBacklog
    {
        Task Store(ErrorPayload payload);
        Task<ValueOrError<Recap>> GetApplicationsRecap(IEnumerable<Application> apps, Func<IEnumerable<ErrorPayload>, int> processor);
    }

    public interface IIdentityProvider
    {
        IIdentity GetIdentity(object request);
    }

    public interface IConnector
    {
        void Connect(string user, string token, Action<string> connector);
        void Disconnect(string token);
    }

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

    public interface IDomainPublisher
    {
        IObservable<Delta<Cluster>> GetClustersSequence();
        IObservable<Delta<Application>> GetApplicationsSequence();
        IObservable<Delta<User>> GetUsersSequence();
        IObservable<Delta<Relationship<Cluster, User>>> GetClusterUsersSequence();
        IObservable<Delta<Relationship<Cluster, Application>>> GetClusterApplicationsSequence();
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
