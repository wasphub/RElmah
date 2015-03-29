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

        private readonly Subject<ErrorPayload> _errors = new Subject<ErrorPayload>();

        public Connection(string endpoint)
        {
            _endpoint = endpoint;
        }

        private Task Connect(HubConnection connection)
        {
            var proxy = _connection.CreateHubProxy("relmah-errors");

            proxy.On<ErrorPayload>(
                "error",
                p => _errors.OnNext(p));

            return connection.Start();
        }

        public Task Start(ClientToken token)
        {
            _connection = new HubConnection(_endpoint, string.Format("user={0}", token.Token));

            return Connect(_connection);
        }

        public Task Start(ICredentials credentials)
        {
            _connection = new HubConnection(_endpoint)
            {
                Credentials = credentials
            };

            return Connect(_connection);
        }

        public Task Start()
        {
            return Start(CredentialCache.DefaultCredentials);
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
