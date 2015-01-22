using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Subscriptions
{
    public class RecapsSubscription : ISubscription
    {
        public IDisposable Subscribe(string user, INotifier notifier, IErrorsInbox errorsInbox, IDomainPersistor domainPersistor, IDomainPublisher domainPublisher)
        {
            var userApps =
                from p in domainPublisher.GetClusterApplicationsSequence()
                let target = p.Target.Secondary.Name
                from u in p.Target.Primary.Users
                where u.Name == user
                select p.Target.Primary.Applications;

            //SOTW
            Func<Task<ValueOrError<Recap>>> initialRecap = async () => await errorsInbox.GetApplicationsRecap(await domainPersistor.GetUserApplications(user));
            var result = initialRecap().Result;
            if (result.HasValue)
                notifier.Recap(user, result.Value);

            return userApps
                .Subscribe(async payload =>
                {
                    var recap = await errorsInbox.GetApplicationsRecap(payload);
                    if (recap.HasValue)
                        notifier.Recap(user, result.Value);
                });
        }
    }
}