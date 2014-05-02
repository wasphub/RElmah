using System;
using System.Reactive.Subjects;
using RElmah.Server.Domain;

namespace RElmah.Server.Services
{
    public class ErrorsInbox : IErrorsInbox
    {
        private readonly Subject<ErrorDescriptor> _errors;

        public ErrorsInbox()
        {
            _errors = new Subject<ErrorDescriptor>();
        }

        public void Post(ErrorDescriptor descriptor)
        {
            _errors.OnNext(descriptor);
        }

        public IObservable<ErrorDescriptor> GetErrors()
        {
            return _errors;
        }
    }
}
