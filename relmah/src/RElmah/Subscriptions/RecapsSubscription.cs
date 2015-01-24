using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

            var userApps =
                from p in domainPublisher.GetClusterApplicationsSequence()
                let target = p.Target.Secondary.Name
                from u in p.Target.Primary.Users
                where u.Name == name
                select p.Target.Primary.Applications;

            //SOTW
            Func<Task<ValueOrError<Recap>>> initialRecap = async () => await errorsInbox.GetApplicationsRecap(await domainPersistor.GetUserApplications(name));
            var result = initialRecap().Result;
            if (result.HasValue)
                notifier.Recap(name, result.Value);

            return userApps
                .Subscribe(async payload =>
                {
                    var recap = await errorsInbox.GetApplicationsRecap(payload);
                    if (recap.HasValue)
                        notifier.Recap(name, result.Value);
                });
        }
    }
}