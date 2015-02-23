using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Extensions;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.StandingQueries
{
    public class RecapsStandingQuery : IStandingQuery
    {
        public async Task<IDisposable> Run(ValueOrError<User> user, IFrontendNotifier frontendNotifier, IErrorsInbox errorsInbox, IErrorsBacklog errorsBacklog,  IDomainPersistor domainPersistor, IDomainPublisher domainPublisher)
        {
            var name = user.Value.Name;

            //Initial recap
            var initialRecap = await InitialRecap(name, domainPersistor, errorsBacklog, (a, r) => new { Applications = a, Recap = r });
            var rs =
                from r in initialRecap.ToSingleton().ToObservable()
                select new
                {
                    r.Applications,
                    Additions = r.Applications,
                    Removals  = Enumerable.Empty<Application>()
                };

            //Deltas
            var clusterApps =
                from p in domainPublisher.GetClusterApplicationsSequence()
                let deltas = p.Target.Secondary.ToSingleton()
                let target = p.Target.Secondary.Name
                from u in p.Target.Primary.Users
                where u.Name == name
                select new
                {
                    p.Target.Primary.Applications,
                    Additions = p.Type == DeltaType.Added   ? deltas : Enumerable.Empty<Application>(),
                    Removals  = p.Type == DeltaType.Removed ? deltas : Enumerable.Empty<Application>()
                };
            var userApps =
                from p in domainPublisher.GetClusterUsersSequence()
                let deltas = p.Target.Primary.Applications
                where p.Target.Secondary.Name == name
                select new
                {
                    Applications = deltas,
                    Additions    = p.Type == DeltaType.Added   ? deltas : Enumerable.Empty<Application>(),
                    Removals     = p.Type == DeltaType.Removed ? deltas : Enumerable.Empty<Application>()
                };
            var apps = clusterApps.Merge(userApps);

            //Stream
            return rs.Concat(apps)
                .Subscribe(async payload =>
                {
                    var recap = await errorsBacklog.GetApplicationsRecap(payload.Applications, xs => xs.Count());
                    if (!recap.HasValue) return;

                    frontendNotifier.Recap(name, recap.Value);
                    frontendNotifier.UserApplications(name, 
                        payload.Additions.Select(a => a.Name), 
                        payload.Removals.Select(a => a.Name));
                });
        }

        static async Task<T> InitialRecap<T>(
            string name, 
            IDomainReader domainPersistor, 
            IErrorsBacklog errorsBacklog,
            Func<IEnumerable<Application>, ValueOrError<Recap>, T> resultor)
        {
            var applications = (await domainPersistor.GetUserApplications(name)).ToArray();

            var recap        = await errorsBacklog.GetApplicationsRecap(applications, xs => xs.Count());

            return resultor(applications, recap);
        }
    }
}