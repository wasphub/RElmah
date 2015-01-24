using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Subscriptions
{
    public class ErrorsSubscription : ISubscription
    {
        public IDisposable Subscribe(ValueOrError<User> user, INotifier notifier, IErrorsInbox errorsInbox, IDomainPersistor domainPersistor, IDomainPublisher domainPublisher)
        {
            if (!user.HasValue || user.Value.Tokens.Count() > 1) return Disposable.Empty;

            var name = user.Value.Name;
            Func<IEnumerable<Application>> getUserApps = () => domainPersistor.GetUserApplications(name).Result;

            var errors =
                from e in errorsInbox.GetErrorsStream()
                from a in getUserApps().ToObservable()
                where e.SourceId == a.Name
                select e;

            return errors
                .Subscribe(payload =>
                {
                    notifier.Error(name, payload);
                });
        }
    }
}