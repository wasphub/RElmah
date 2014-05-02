using System;
using System.Reactive.Subjects;
using RElmah.Server.Domain;

namespace RElmah.Server.Services
{
    public class ErrorsInbox : IErrorsInbox
    {
        private readonly IErrorsBacklog _backlog;
        private readonly Subject<ErrorPayload> _errors;

        public ErrorsInbox() : this(new NullErrorsBacklog())
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
