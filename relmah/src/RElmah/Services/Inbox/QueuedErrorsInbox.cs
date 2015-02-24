using System;
using System.Collections.Concurrent;
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
    public class QueuedErrorsInbox : IErrorsInbox
    {
        private readonly IErrorsBacklog _errorsBacklog;

        private readonly BlockingCollection<ErrorPayload> _queue = new BlockingCollection<ErrorPayload>(new ConcurrentQueue<ErrorPayload>());

        private readonly Subject<ErrorPayload> _errors;
        private readonly IObservable<ErrorPayload> _publishedErrors;

        public QueuedErrorsInbox() : this(NullErrorsBacklog.Instance) { }

        public QueuedErrorsInbox(IErrorsBacklog errorsBacklog)
        {
            _errorsBacklog = errorsBacklog;

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

        public Task<ValueOrError<Recap>> GetApplicationsRecap(IEnumerable<Application> apps)
        {
            return _errorsBacklog.GetApplicationsRecap(apps, xs => xs.Count());
        }
    }
}