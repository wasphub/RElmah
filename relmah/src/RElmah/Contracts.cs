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

    public interface IDomainWriter
    {
        Task<ValueOrError<Cluster>> AddCluster(string name);
        Task<ValueOrError<bool>> RemoveCluster(string name);
        Task<IEnumerable<Cluster>> GetClusters();
        Task<ValueOrError<Cluster>> GetCluster(string name);
        Task<ValueOrError<Application>> AddApplication(string name);
        Task<ValueOrError<bool>> RemoveApplication(string name);
        Task<IEnumerable<Application>> GetApplications();
        Task<ValueOrError<Application>> GetApplication(string name);
        Task<ValueOrError<User>> AddUser(string name);
        Task<ValueOrError<bool>> RemoveUser(string name);
        Task<IEnumerable<User>> GetUsers();
        Task<ValueOrError<User>> GetUser(string name);
        Task<ValueOrError<Relationship<Cluster, User>>> AddUserToCluster(string cluster, string user);
        Task<ValueOrError<Relationship<Cluster, User>>> RemoveUserFromCluster(string cluster, string user);
        Task<ValueOrError<Relationship<Cluster, Application>>> AddApplicationToCluster(string cluster, string application);
        Task<ValueOrError<Relationship<Cluster, Application>>> RemoveApplicationFromCluster(string cluster, string application);
        Task<IEnumerable<Application>> GetUserApplications(string user);
        Task<ValueOrError<User>> AddUserToken(string user, string token);
        Task<ValueOrError<User>> RemoveUserToken(string token);
    }

    public interface IDomainStore : IDomainWriter
    {
    }

    public interface IDomainReader
    {
        IObservable<Delta<Cluster>> ObserveClusters();
        IObservable<Delta<Application>> ObserveApplications();
        IObservable<Delta<User>> ObserveUsers();
        IObservable<Delta<Relationship<Cluster, User>>> ObserveClusterUsers();
        IObservable<Delta<Relationship<Cluster, Application>>> ObserveClusterApplications();
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
