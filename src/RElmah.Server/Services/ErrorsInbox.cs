using System;
using System.Reactive.Subjects;
using RElmah.Server.Domain;
using RElmah.Server.Services.Nulls;

namespace RElmah.Server.Services
{
    public class ErrorsInbox : IErrorsInbox
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly Subject<ErrorPayload> _errors;
        private readonly IErrorsBacklog _backlog;

        public ErrorsInbox(IConfigurationProvider configurationProvider)
            : this(new NullErrorsBacklog(), new NullDispatcher(), configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public ErrorsInbox(IDispatcher dispatcher, IConfigurationProvider configurationProvider)
            : this(new NullErrorsBacklog(), dispatcher, configurationProvider)
        {
        }

        public ErrorsInbox(IErrorsBacklog backlog, IDispatcher dispatcher, IConfigurationProvider configurationProvider)
        {
            _backlog = backlog;
            _errors = new Subject<ErrorPayload>();
            _errors.Subscribe(p => dispatcher.DispatchError(configurationProvider, p));
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
