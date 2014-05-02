using System;
using System.Reactive.Subjects;
using RElmah.Server.Domain;

namespace RElmah.Server.Services
{
    public class ErrorsInbox : IErrorsInbox
    {
        private readonly IErrorsBacklog _backlog;
        private readonly Subject<ErrorDescriptor> _errors;

        public ErrorsInbox() : this(new NullErrorsBacklog())
        {
        }

        public ErrorsInbox(IErrorsBacklog backlog)
        {
            _backlog = backlog;
            _errors = new Subject<ErrorDescriptor>();
        }

        public void Post(ErrorDescriptor descriptor)
        {
            _backlog.Store(descriptor);
            _errors.OnNext(descriptor);
        }

        public IObservable<ErrorDescriptor> GetErrors()
        {
            return _errors;
        }
    }
}
