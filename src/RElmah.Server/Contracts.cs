using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Domain;
using RElmah.Server.Infrastructure;

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
    }

    public interface IDispatcher
    {
        Task DispatchError(ErrorPayload payload);
        Task DispatchClusterOperation(Operation<Cluster> op);
        Task DispatchApplicationOperation(Operation<Application> op);
    }

    public interface IConfigurationProvider
    {
        IEnumerable<string> ExtractGroups(ErrorPayload payload);
        IEnumerable<Cluster> Clusters { get; }
        IEnumerable<Application> Applications { get; }
        void AddCluster(string cluster);
        Cluster GetCluster(string name);
        void AddApplication(string name, string sourceId, string cluster);
        Application GetApplication(string name);
    }

    public interface IConfigurationDispatcher
    {
        Task Dispatch(Cluster cluster);
    }
}
