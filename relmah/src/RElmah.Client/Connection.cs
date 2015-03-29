using System;
using System.Net;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using RElmah.Common;

namespace RElmah.Client
{
    public class Connection : IDisposable
    {
        private readonly string _endpoint;

        private HubConnection _connection;

        private ISubject<ErrorPayload> _errors = new Subject<ErrorPayload>();

        public Connection(string endpoint)
        {
            _endpoint = endpoint;
        }

        public Task Start(ConnectionOptions options, ClientToken token)
        {
            _connection = new HubConnection(_endpoint, string.Format("user={0}", token.Token));

            ApplyOptions(options);

            return Connect(_connection);
        }

        public Task Start(ClientToken token)
        {
            return Start(null, token);
        }

        public Task Start(ConnectionOptions options, ICredentials credentials)
        {
            _connection = new HubConnection(_endpoint)
            {
                Credentials = credentials
            };

            ApplyOptions(options);

            return Connect(_connection);
        }

        public Task Start(ICredentials credentials)
        {
            return Start(null, credentials);
        }

        public Task Start(ConnectionOptions options)
        {
            ApplyOptions(options);

            return Start(options, CredentialCache.DefaultCredentials);
        }

        public Task Start()
        {
            return Start((ConnectionOptions)null);
        }

        private void ApplyOptions(ConnectionOptions options)
        {
            if (options != null)
            {
                if (options.ErrorsFilter != null)
                {
                    var disposable = _errors as IDisposable;
                    if (disposable != null) disposable.Dispose();

                    _errors = new FilteredSubject<ErrorPayload>(options.ErrorsFilter);
                }
            }

        }

        private Task Connect(HubConnection connection)
        {
            var proxy = connection.CreateHubProxy("relmah-errors");

            proxy.On<ErrorPayload>(
                "error",
                p => _errors.OnNext(p));

            return connection.Start().ContinueWith(t => { var foo = _connection.State; });
        }

        public IObservable<ErrorPayload> Errors { get { return _errors; } }

        public void Dispose()
        {
            _connection.Dispose();

            _errors.OnCompleted();

            var disposable = _errors as IDisposable;
            if (disposable != null) disposable.Dispose();
        }
    }
}
