using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Errors;
using RElmah.Foundation;
using RElmah.Models;
using RElmah.Services.Nulls;

namespace RElmah.Services.Inbox
{
    public class SerializedErrorsInbox : IErrorsInbox
    {
        private readonly IErrorsBacklog _errorsBacklog;

        private readonly Subject<ErrorPayload> _errors;
        private readonly IObservable<ErrorPayload> _publishedErrors;

        public SerializedErrorsInbox() : this(NullErrorsBacklog.Instance) { }

        public SerializedErrorsInbox(IErrorsBacklog errorsBacklog)
        {
            _errorsBacklog = errorsBacklog;

            _errors = new Subject<ErrorPayload>();
            _publishedErrors = _errors.Publish().RefCount();
        }

        public Task Post(ErrorPayload payload)
        {
            return _errorsBacklog
                .Store(payload)
                .ContinueWith(_ => _errors.OnNext(payload));
        }

        public IObservable<ErrorPayload> GetErrorsStream()
        {
            return _publishedErrors;
        }

        public Task<ValueOrError<Recap>> GetSourcesRecap(IEnumerable<Source> sources, Func<IEnumerable<ErrorPayload>, int> reducer)
        {
            return _errorsBacklog.GetSourcesRecap(sources, xs => xs.Count());
        }
    }
}
