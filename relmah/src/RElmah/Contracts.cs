using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;
using RElmah.Models.Errors;

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
    }

    /// <summary>
    /// This contract represents an errors
    /// dispatcher, which is able to route
    /// errors to the right destination(s)
    /// as soon as they get in.
    /// </summary>
    public interface IDispatcher
    {
        Task DispatchError(ErrorPayload payload);
    }

    public interface IConfigurationUpdater
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
    }

    public interface IConfigurationProvider
    {
        IObservable<Delta<Cluster>> ObserveClusters();
        IObservable<Delta<Application>> ObserveApplications();
        IObservable<Delta<User>> ObserveUsers();
    }
}
