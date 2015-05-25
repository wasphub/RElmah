using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common.Model;
using RElmah.Foundation;

namespace RElmah.Queries.Frontend
{
    public class ErrorsQuery : IFrontendQuery
    {
        public async Task<IDisposable> Run(ValueOrError<User> user, RunTargets targets)
        {
            if (user.Value.Tokens.Count() > 1) return Disposable.Empty;

            var name = user.Value.Name;
            Func<Task<IEnumerable<Source>>> getUserSources = async () => await targets.VisibilityPersistor.GetUserSources(name);

            var frontend = targets.FrontendErrorsInbox != null 
                         ? targets.FrontendErrorsInbox.GetErrorsStream() 
                         : Observable.Empty<ErrorPayload>();
            var backend  = targets.BackendErrorsInbox != null 
                         ? targets.BackendErrorsInbox.GetErrorsStream() 
                         : Observable.Empty<ErrorPayload>();
            var errors =
                from e in frontend.Merge(backend)
                from sources in getUserSources()
                from a in sources
                where e.SourceId == a.SourceId
                select e;

            return errors.Subscribe(payload => targets.FrontendNotifier.Error(name, payload));
        }
    }
}