using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Server.Domain;

namespace RElmah.Server
{
    public interface IErrorsInbox
    {
        void Post(ErrorPayload payload);
        IObservable<ErrorPayload> GetErrors();
    }

    public interface IErrorsBacklog
    {
        Task Store(ErrorPayload payload);
        Task<IEnumerable<ErrorPayload>> GetErrors();
    }

    public interface IDispatcher
    {
        Task DispatchError(IConfigurationProvider configurationProvider, ErrorPayload payload);
        Task DispatchClusterOperation(IConfigurationProvider configurationProvider, Operation<Cluster> op);
        Task DispatchApplicationOperation(IConfigurationProvider configurationProvider, Operation<Application> op);
    }

    public interface IConfigurationProvider
    {
        T ExtractInfo<T>(ErrorPayload payload, Func<string, string, T> resultor);
        IEnumerable<Cluster> Clusters { get; }
        IEnumerable<Application> Applications { get; }
        void AddCluster(string cluster);
        Cluster GetCluster(string name);
        IObservable<Operation<Cluster>> GetObservableClusters();
        void AddApplication(string name, string sourceId, string cluster);
        Application GetApplication(string name);
        IObservable<Operation<Application>> GetObservableApplications();
        IEnumerable<Application> GetVisibleApplications(IPrincipal user);
        void AddUserToCluster(string user, string cluster);
    }
}
