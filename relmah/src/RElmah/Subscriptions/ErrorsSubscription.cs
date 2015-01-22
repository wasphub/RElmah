using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using RElmah.Models;

namespace RElmah.Subscriptions
{
    public class ErrorsSubscription : ISubscription
    {
        public IDisposable Subscribe(string user, INotifier notifier, IErrorsInbox errorsInbox, IDomainPersistor domainPersistor, IDomainPublisher domainPublisher)
        {
            Func<IEnumerable<Application>> getUserApps = () => domainPersistor.GetUserApplications(user).Result;

            var errors =
                from e in errorsInbox.GetErrorsStream()
                from a in getUserApps().ToObservable()
                where e.SourceId == a.Name
                select e;

            return errors
                .Subscribe(payload => notifier.Error(user, payload));
        }
    }
}