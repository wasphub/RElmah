using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Extensions;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Subscriptions
{
    public class RecapsSubscription : ISubscription
    {
        public IDisposable Subscribe(ValueOrError<User> user, INotifier notifier, IErrorsInbox errorsInbox, IDomainPersistor domainPersistor, IDomainPublisher domainPublisher)
        {
            if (!user.HasValue) return Disposable.Empty;

            var name = user.Value.Name;

            var clusterApps =
                from p in domainPublisher.GetClusterApplicationsSequence()
                let target = p.Target.Secondary.Name
                from u in p.Target.Primary.Users
                where u.Name == name
                select new
                {
                    p.Target.Primary.Applications,
                    Additions = p.Type == DeltaType.Added   ? p.Target.Secondary.ToSingleton() : Enumerable.Empty<Application>(),
                    Removals  = p.Type == DeltaType.Removed ? p.Target.Secondary.ToSingleton() : Enumerable.Empty<Application>()
                };
            var userApps =
                from p in domainPublisher.GetClusterUsersSequence()
                where p.Target.Secondary.Name == name
                select new
                {
                    p.Target.Primary.Applications,
                    Additions = p.Type == DeltaType.Added   ? p.Target.Primary.Applications : Enumerable.Empty<Application>(),
                    Removals  = p.Type == DeltaType.Removed ? p.Target.Primary.Applications : Enumerable.Empty<Application>()
                };
            var apps = clusterApps.Merge(userApps);

            //Initial recap
            Func<Task<ValueOrError<Recap>>> initialRecap = async () => await errorsInbox.GetApplicationsRecap(await domainPersistor.GetUserApplications(name));
            var result = initialRecap().Result;
            if (result.HasValue)
                notifier.Recap(name, result.Value);

            return apps
                .Subscribe(async payload =>
                {
                    var recap = await errorsInbox.GetApplicationsRecap(payload.Applications);
                    if (recap.HasValue)
                    {
                        notifier.Recap(name, result.Value);
                        notifier.UserApplications(name, payload.Additions.Select(a => a.Name), payload.Removals.Select(a => a.Name));
                    }
                });
        }
    }
}