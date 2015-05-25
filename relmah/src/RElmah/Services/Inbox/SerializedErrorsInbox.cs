using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Common.Model;
using RElmah.Errors;
using RElmah.Services.Nulls;

namespace RElmah.Services.Inbox
{
    public class SerializedErrorsInbox : IErrorsInbox
    {
        private readonly IErrorsBacklog _errorsBacklog;
        private readonly int _bufferSize;
        private readonly int _bufferPeriod;

        private readonly Subject<ErrorPayload> _errors;
        private readonly IObservable<ErrorPayload> _publishedErrors;

        public SerializedErrorsInbox() : this(NullErrorsBacklog.Instance, 30, 1) { }

        public SerializedErrorsInbox(IErrorsBacklog errorsBacklog, int bufferSize, int bufferPeriod)
        {
            _errorsBacklog = errorsBacklog;

            _bufferSize = bufferSize;
            _bufferPeriod = bufferPeriod;

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

        public IObservable<IList<ErrorPayload>> GetErrorBuffersStream()
        {
            return _publishedErrors.Buffer(
                TimeSpan.FromSeconds(_bufferSize),
                TimeSpan.FromSeconds(_bufferPeriod));
        }
    }
}
