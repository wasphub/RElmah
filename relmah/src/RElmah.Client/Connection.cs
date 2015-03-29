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
        private readonly HubConnection _connection;

        private readonly Subject<ErrorPayload> _errors = new Subject<ErrorPayload>();

        public Connection(string endpoint, ClientToken token)
        {
            _connection = new HubConnection(endpoint, string.Format("user={0}", token.Token));

            SetupProxy();
        }

        public Connection(string endpoint, ICredentials credentials)
        {
            _connection = new HubConnection(endpoint)
            {
                Credentials = credentials
            };

            SetupProxy();
        }

        private void SetupProxy()
        {
            var proxy = _connection.CreateHubProxy("relmah-errors");

            proxy.On<ErrorPayload>(
                "error",
                p => _errors.OnNext(p));
        }

        public Connection(string endpoint) : this(endpoint, CredentialCache.DefaultCredentials) { }

        public Task Start()
        {
            return _connection.Start().ContinueWith(t => { var foo = _connection.State; });
        }

        public IObservable<ErrorPayload> Errors { get { return _errors; } }

        public void Dispose()
        {
            _connection.Dispose();

            _errors.OnCompleted();
            _errors.Dispose();
        }
    }
}
