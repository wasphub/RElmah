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
        public IDisposable Run(ValueOrError<User> user, INotifier notifier, IErrorsInbox errorsInbox, IDomainPersistor domainPersistor, IDomainPublisher domainPublisher)
        {
            var name = user.Value.Name;

            //Initial recap
            var initialRecap = InitialRecap(name, domainPersistor, errorsInbox, (a, r) => new { Applications = a, Recap = r });
            var rs =
                from i in initialRecap.Result.ToSingleton().ToObservable()
                select new
                {
                    i.Applications,
                    Additions = i.Applications,
                    Removals = Enumerable.Empty<Application>()
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
                    var recap = await errorsInbox.GetApplicationsRecap(payload.Applications);
                    if (!recap.HasValue) return;

                    notifier.Recap(name, recap.Value);
                    notifier.UserApplications(name, 
                        payload.Additions.Select(a => a.Name), 
                        payload.Removals.Select(a => a.Name));
                });
        }

        static async Task<T> InitialRecap<T>(
            string name, 
            IDomainReader domainPersistor, 
            IErrorsInbox errorsInbox, 
            Func<IEnumerable<Application>, ValueOrError<Recap>, T> resultor)
        {
            var applications = await domainPersistor.GetUserApplications(name);
            applications = applications.ToArray();
            var recap = await errorsInbox.GetApplicationsRecap(applications);
            return resultor(applications, recap);
        }
    }
}