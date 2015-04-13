using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Common.Extensions;
using RElmah.Common.Model;
using RElmah.Errors;
using RElmah.Foundation;
using RElmah.Visibility;

namespace RElmah.Queries.Frontend
{
    public class RecapsQuery : IFrontendQuery
    {
        public async Task<IDisposable> Run(ValueOrError<User> user, RunTargets targets)
        {
            var name = user.Value.Name;

            //Initial recap
            var initialRecap = await InitialRecap(name, targets.VisibilityPersistor, targets.ErrorsBacklog, (a, r) => new { Sources = a, Recap = r });
            var rs =
                from r in initialRecap.ToSingleton().ToObservable()
                select new
                {
                    Sources   = r.Sources,
                    Additions = r.Sources,
                    Removals  = Enumerable.Empty<Source>()
                };

            //Deltas

            var clusterSources =
                from p in targets.VisibilityPublisher.GetClusterSourcesSequence()
                let sources = p.Target.Primary.Sources
                let deltas = p.Target.Secondary.ToSingleton()
                let target = p.Target.Secondary.SourceId
                from u in p.Target.Primary.Users
                where u.Name == name
                select new
                {
                    Sources   = sources,
                    Additions = p.Type == DeltaType.Added   ? deltas : Enumerable.Empty<Source>(),
                    Removals  = p.Type == DeltaType.Removed ? deltas : Enumerable.Empty<Source>()
                };

            var userSources =
                from p in targets.VisibilityPublisher.GetClusterUsersSequence()
                let sources = p.Target.Primary.Sources
                where p.Target.Secondary.Name == name
                select new
                {
                    Sources   = sources,
                    Additions = p.Type == DeltaType.Added   ? sources : Enumerable.Empty<Source>(),
                    Removals  = p.Type == DeltaType.Removed ? sources : Enumerable.Empty<Source>()
                };

            var mergedSources = clusterSources.Merge(userSources);

            //Stream
            return rs.Concat(mergedSources).Subscribe(async payload =>
            {
                var recap = await targets.ErrorsBacklog.GetSourcesRecap(payload.Sources, xs => xs.Count());
                if (!recap.HasValue) return;

                targets.FrontendNotifier.Recap(name, recap.Value);
                targets.FrontendNotifier.UserSources(name,
                    payload.Additions, payload.Removals);
            });
        }

        static async Task<T> InitialRecap<T>(
            string name, 
            IVisibilityAccessor visibilityPersistor, 
            IErrorsBacklog errorsBacklog,
            Func<IEnumerable<Source>, ValueOrError<Recap>, T> resultor)
        {
            var sources = (await visibilityPersistor.GetUserSources(name)).ToArray();

            var recap   = await errorsBacklog.GetSourcesRecap(sources, xs => xs.Count());

            return resultor(sources, recap);
        }
    }
}