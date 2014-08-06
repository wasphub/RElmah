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
        Task DispatchClusterUserOperation(IConfigurationProvider configurationProvider, Delta<ClusterUser> op);
    }

    public interface IConfigurationProvider
    {
        T ExtractInfo<T>(ErrorPayload payload, Func<string, string, T> resultor);

        IEnumerable<Cluster> Clusters { get; }
        IObservable<Delta<Cluster>> GetClustersDeltas();
        IConfigurationProvider AddCluster(string cluster);
        Cluster GetCluster(string name);
        IEnumerable<Application> Applications { get; }
        IObservable<Delta<Application>> GetApplicationsDeltas();
        IConfigurationProvider AddApplication(string sourceId, string name, string cluster);
        Application GetApplication(string name);
        IEnumerable<Application> GetVisibleApplications(IPrincipal user);
        IConfigurationProvider AddUserToCluster(string user, string cluster);
        IConfigurationProvider RemoveUserFromCluster(string user, string cluster);
        IObservable<Delta<ClusterUser>> GetClusterUserDeltas();
    }
}
