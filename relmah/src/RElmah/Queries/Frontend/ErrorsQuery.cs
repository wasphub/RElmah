using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Queries.Frontend
{
    public class ErrorsQuery : IFrontendQuery
    {
        public async Task<IDisposable> Run(ValueOrError<User> user, RunTargets targets)
        {
            if (user.Value.Tokens.Count() > 1) return Disposable.Empty;

            var name = user.Value.Name;
            Func<Task<IEnumerable<Application>>> getUserApps = async () => await targets.DomainPersistor.GetUserApplications(name);

            var frontend = targets.ErrorsInbox != null 
                         ? targets.ErrorsInbox.GetErrorsStream() 
                         : Observable.Empty<ErrorPayload>();
            var backend  = targets.BackendErrorsInbox != null 
                         ? targets.BackendErrorsInbox.GetErrorsStream() 
                         : Observable.Empty<ErrorPayload>();
            var errors =
                from e in frontend.Merge(backend)
                from apps in getUserApps()
                from a in apps
                where e.SourceId == a.Name
                select e;

            return errors
                .Subscribe(payload =>
                {
                    targets.FrontendNotifier.Error(name, payload);
                });
        }
    }
}