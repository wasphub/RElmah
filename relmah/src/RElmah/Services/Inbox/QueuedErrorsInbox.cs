using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RElmah.Common.Model;
using RElmah.Errors;
using RElmah.Services.Nulls;
using System.Collections.Generic;
using System.Diagnostics;

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
        private readonly IObservable<IList<ErrorPayload>> _publishedErrorBuffers;

        public QueuedErrorsInbox() : this(NullErrorsBacklog.Instance, 30, 1) { }

        public QueuedErrorsInbox(int bufferSize, int bufferPeriod) : this(NullErrorsBacklog.Instance, bufferSize, bufferPeriod) { }

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

            _errors                = new Subject<ErrorPayload>();
            _publishedErrors       = _errors.Publish().RefCount();
            var connectable        = GenerateErrorBuffersStream().Publish();
            _publishedErrorBuffers = connectable;
            connectable.Connect();
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
            return _publishedErrorBuffers;
        }

        IObservable<IList<ErrorPayload>> GenerateErrorBuffersStream()
        {
            var running =
                _errors.Buffer(TimeSpan.FromSeconds(_bufferSize), TimeSpan.FromSeconds(_bufferPeriod))
                       .Do(onNext: es => { Trace.WriteLine(string.Format("running -> {0}", es.Count)); });

            var gen =
                Observable.Range(1, _bufferSize / _bufferPeriod)
                          .SelectMany(x => _errors.Buffer(TimeSpan.FromSeconds(x * _bufferPeriod), TimeSpan.FromSeconds(_bufferPeriod))
                                                  .Take(1)
                                                  .Do(onNext: es => { Trace.WriteLine(string.Format("gen {0} -> {1}", x, es.Count)); }));
            var intro = Observable.Concat(gen);

            var pi = intro.Publish();
            pi.Connect();

            var pr = running.Publish();
            pr.Connect();

            return pi.Concat(pr);
        }
    }
}