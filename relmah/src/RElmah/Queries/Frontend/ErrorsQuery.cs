using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RElmah.Common.Extensions;
using RElmah.Common.Model;
using RElmah.Errors;
using RElmah.Extensions;
using RElmah.Foundation;
using System.Diagnostics;

namespace RElmah.Queries.Frontend
{
    public class ErrorsQuery : IFrontendQuery
    {
        public async Task<IDisposable> Run(ValueOrError<User> user, RunTargets targets)
        {
            var name = user.Value.Name;
            Func<Task<IEnumerable<Source>>> getUserSources = async () => await targets.VisibilityPersistor.GetUserSources(name);

            Func<IErrorsInbox, IObservable<ErrorPayload>> bufferGen = ei => 
                from i in Observable.Return(ei)
                where i != null
                from ss in i.GetErrorBuffersStream().Take(1)
                from s in ss
                select s;

            Func<IErrorsInbox, IObservable<ErrorPayload>> streamGen = ei => ei.GetErrorsStream();

            var frontend = user.Value.Tokens.Count() == 1
                         ? bufferGen(targets.FrontendErrorsInbox).Concat(streamGen(targets.FrontendErrorsInbox))
                         : bufferGen(targets.FrontendErrorsInbox);
            var backend  = user.Value.Tokens.Count() == 1
                         ? bufferGen(targets.BackendErrorsInbox).Concat(streamGen(targets.BackendErrorsInbox))
                         : bufferGen(targets.BackendErrorsInbox);

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