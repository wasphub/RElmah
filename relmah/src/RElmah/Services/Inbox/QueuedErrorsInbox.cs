using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Common.Model;
using RElmah.Errors;
using RElmah.Services.Nulls;
using System.Collections.Generic;

namespace RElmah.Services.Inbox
{
    public class QueuedErrorsInbox : IErrorsInbox
    {
        private readonly IErrorsBacklogWriter _errorsBacklog;
        private readonly int _bufferSize;
        private readonly int _bufferPeriod;

        private readonly BlockingCollection<ErrorPayload> _queue = new BlockingCollection<ErrorPayload>(new ConcurrentQueue<ErrorPayload>());

        private readonly Subject<ErrorPayload> _errors;
        private readonly IObservable<ErrorPayload> _publishedErrors;

        public QueuedErrorsInbox() : this(NullErrorsBacklog.Instance, 30, 1) { }

        public QueuedErrorsInbox(IErrorsBacklogWriter errorsBacklog, int bufferSize, int bufferPeriod)
        {
            _errorsBacklog = errorsBacklog;

            _bufferSize = bufferSize;
            _bufferPeriod = bufferPeriod;

            Task.Run(() =>
            {
                //it will block here automatically waiting from new items to be added and it will not take cpu down 
                foreach (var data in _queue.GetConsumingEnumerable())
                {
                    _errors.OnNext(data); 
                }
            });

            _errors = new Subject<ErrorPayload>();
            _publishedErrors = _errors.Publish().RefCount();
        }

        public Task Post(ErrorPayload payload)
        {
            return _errorsBacklog
                .Store(payload)
                .ContinueWith(_ => _queue.Add(payload));
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