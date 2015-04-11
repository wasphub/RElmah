using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RElmah.Queries.Backend
{
    public class VisibilityBusQuery : IBackendQuery
    {
        private readonly bool _skipEventsFromBackend;

        public VisibilityBusQuery(bool skipEventsFromBackend = false)
        {
            _skipEventsFromBackend = skipEventsFromBackend;
        }

        public Task<IDisposable> Run(RunTargets targets)
        {
            var clusters = targets.VisibilityPublisher.GetClustersSequence()
                .Where(p => !_skipEventsFromBackend || !p.FromBackend)
                .Subscribe(payload =>
            {
                targets.BackendNotifier.Cluster(payload);
            });

            var sources = targets.VisibilityPublisher.GetSourcesSequence()
                .Where(p => !_skipEventsFromBackend || !p.FromBackend)
                .Subscribe(payload =>
            {
                targets.BackendNotifier.Source(payload);
            });

            var users = targets.VisibilityPublisher.GetUsersSequence()
                .Where(p => !_skipEventsFromBackend || !p.FromBackend)
                .Subscribe(payload =>
            {
                targets.BackendNotifier.User(payload);
            });

            var clusterSources = targets.VisibilityPublisher.GetClusterSourcesSequence()
                .Where(p => !_skipEventsFromBackend || !p.FromBackend)
                .Subscribe(payload =>
            {
                targets.BackendNotifier.ClusterSource(payload);
            });

            var clusterUsers = targets.VisibilityPublisher.GetClusterUsersSequence()
                .Where(p => !_skipEventsFromBackend || !p.FromBackend)
                .Subscribe(payload =>
            {
                targets.BackendNotifier.ClusterUser(payload);
            });

            return Task.FromResult((IDisposable)new CompositeDisposable(clusters, sources, users, clusterSources, clusterUsers));
        }
    }
}