using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Models.Errors;

namespace RElmah.Services
{
    public class ErrorsInbox : IErrorsInbox
    {
        private readonly Subject<ErrorPayload> _errors = new Subject<ErrorPayload>();

        public Task Post(ErrorPayload payload)
        {
            return Task.Factory.StartNew(() => _errors.OnNext(payload));
        }

        public IObservable<ErrorPayload> GetErrorsStream()
        {
            return _errors;
        }
    }
}
