using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RElmah.Queries.Backend
{
    public class VisibilityQuery : IBackendQuery
    {
        private readonly bool _skipEventsFromBackend;

        public VisibilityQuery(bool skipEventsFromBackend = false)
        {
            _skipEventsFromBackend = skipEventsFromBackend;
        }

        public Task<IDisposable> Run(RunTargets targets)
        {
            Func<bool, bool> filter = p => !_skipEventsFromBackend || !p;

            var clusters = targets.VisibilityPublisher.GetClustersSequence()
                .Where(p => filter(p.FromBackend))
                .Subscribe(payload => targets.BackendNotifier.Cluster(payload));

            var sources = targets.VisibilityPublisher.GetSourcesSequence()
                .Where(p => filter(p.FromBackend))
                .Subscribe(payload => targets.BackendNotifier.Source(payload));

            var users = targets.VisibilityPublisher.GetUsersSequence()
                .Where(p => filter(p.FromBackend))
                .Subscribe(payload => targets.BackendNotifier.User(payload));

            var clusterSources = targets.VisibilityPublisher.GetClusterSourcesSequence()
                .Where(p => filter(p.FromBackend))
                .Subscribe(payload => targets.BackendNotifier.ClusterSource(payload));

            var clusterUsers = targets.VisibilityPublisher.GetClusterUsersSequence()
                .Where(p => filter(p.FromBackend))
                .Subscribe(payload => targets.BackendNotifier.ClusterUser(payload));

            return Task.FromResult((IDisposable)new CompositeDisposable(clusters, sources, users, clusterSources, clusterUsers));
        }
    }
}