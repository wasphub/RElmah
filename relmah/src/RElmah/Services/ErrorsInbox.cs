using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Models.Errors;

namespace RElmah.Services
{
    public class ErrorsInbox : IErrorsInbox
    {
        private readonly IErrorsBacklog _errorsBacklog;

        private readonly Subject<ErrorPayload> _errors = new Subject<ErrorPayload>();

        class NullErrorsBacklog : IErrorsBacklog
        {
            public Task Store(ErrorPayload payload)
            {
                return Task.FromResult((object)null);
            }
        }

        public ErrorsInbox() : this(new NullErrorsBacklog())
        {
        }

        public ErrorsInbox(IErrorsBacklog errorsBacklog)
        {
            _errorsBacklog = errorsBacklog;
        }

        public Task Post(ErrorPayload payload)
        {
            return _errorsBacklog
                .Store(payload)
                .ContinueWith(_ => _errors.OnNext(payload));
        }

        public IObservable<ErrorPayload> GetErrorsStream()
        {
            return _errors;
        }
    }
}
