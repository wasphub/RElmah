using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace RElmah.Queries.Backend
{
    public class ConfigurationBusQuery : IBackendQuery
    {
        public Task<IDisposable> Run(RunTargets targets)
        {
            var clusters = targets.DomainPublisher.GetClustersSequence().Subscribe(payload =>
            {
                targets.BackendNotifier.Cluster(payload);
            });

            var applications = targets.DomainPublisher.GetApplicationsSequence().Subscribe(payload =>
            {
                targets.BackendNotifier.Application(payload);
            });

            var users = targets.DomainPublisher.GetUsersSequence().Subscribe(payload =>
            {
                targets.BackendNotifier.User(payload);
            });

            var clusterApplications = targets.DomainPublisher.GetClusterApplicationsSequence().Subscribe(payload =>
            {
                targets.BackendNotifier.ClusterApplication(payload);
            });

            var clusterUsers = targets.DomainPublisher.GetClusterUsersSequence().Subscribe(payload =>
            {
                targets.BackendNotifier.ClusterUser(payload);
            });

            return Task.FromResult((IDisposable)new CompositeDisposable(clusters, applications, users, clusterApplications, clusterUsers));
        }
    }
}