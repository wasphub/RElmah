using System;
using System.Reactive.Subjects;
using RElmah.Server.Domain;
using RElmah.Server.Services.Nulls;

namespace RElmah.Server.Services
{
    public class ErrorsInbox : IErrorsInbox
    {
        private readonly Subject<ErrorPayload> _errors;
        private readonly IErrorsBacklog _backlog;

        public ErrorsInbox()
            : this(new NullErrorsBacklog())
        {
        }

        public ErrorsInbox(IErrorsBacklog backlog)
        {
            _backlog = backlog;
            _errors = new Subject<ErrorPayload>();
        }

        public void Post(ErrorPayload payload)
        {
            _backlog.Store(payload);
            _errors.OnNext(payload);
        }

        public IObservable<ErrorPayload> GetErrors()
        {
            return _errors;
        }
    }
}
