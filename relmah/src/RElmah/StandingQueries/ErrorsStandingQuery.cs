using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.StandingQueries
{
    public class ErrorsStandingQuery : IStandingQuery
    {
        public async Task<IDisposable> Run(ValueOrError<User> user, INotifier notifier, IErrorsInbox errorsInbox, IDomainPersistor domainPersistor, IDomainPublisher domainPublisher)
        {
            if (user.Value.Tokens.Count() > 1) return Disposable.Empty;

            var name = user.Value.Name;
            Func<Task<IEnumerable<Application>>> getUserApps = async () => await domainPersistor.GetUserApplications(name);

            var errors =
                from e in errorsInbox.GetErrorsStream()
                from apps in getUserApps()
                from a in apps
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