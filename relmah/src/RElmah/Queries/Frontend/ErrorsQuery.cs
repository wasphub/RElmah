using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common.Extensions;
using RElmah.Common.Model;
using RElmah.Errors;
using RElmah.Extensions;
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

            Func<IErrorsInbox, IObservable<ErrorPayload>> bufferThenStreamGen = ei =>
                Observable.Return(ei)
                          .Where(i => i != null)
                          .SelectMany(i => i.GetErrorBuffersStream()
                                            .Take(1)
                                            .SelectMany(es => i.GetErrorsStream()
                                                              .Where(s => !es.Select(e => e.ErrorId).Contains(s.ErrorId))
                                                              .StartWith(es)));

            var frontend = bufferThenStreamGen(targets.FrontendErrorsInbox);

            var backend  = bufferThenStreamGen(targets.BackendErrorsInbox);

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