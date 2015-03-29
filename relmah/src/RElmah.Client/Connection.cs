using System;
using System.Net;
using System.Reactive.Linq;
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

        public IObservable<ErrorPayload> Errors { get { return _errors; } }
        public IObservable<IGroupedObservable<ErrorType, ErrorPayload>> ErrorTypes { get; private set; }

        public Task Start(ConnectionOptions options, ClientToken token)
        {
            _connection = new HubConnection(_endpoint, string.Format("user={0}", token.Token));

            ApplyOptions(options);

            return Connect(_connection);
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

        public Task Start(ClientToken token)
        {
            return Start(null, token);
        }

        public Task Start(ICredentials credentials)
        {
            return Start(null, credentials);
        }

        public Task Start(ConnectionOptions options)
        {
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

        public class ErrorType
        {
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((SourceId != null ? SourceId.GetHashCode() : 0)*397) ^ (Type != null ? Type.GetHashCode() : 0);
                }
            }

            public string SourceId;
            public string Type;

            public bool Equals(ErrorType target)
            {
                if (target == null) return false;
                return string.Equals(SourceId, target.SourceId) && string.Equals(Type, target.Type);
            }
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is ErrorType && Equals((ErrorType) obj);
            }
        }

        private Task Connect(HubConnection connection)
        {
            var proxy = connection.CreateHubProxy("relmah-errors");

            ErrorTypes = _errors.GroupBy(e => new ErrorType {SourceId = e.SourceId, Type = e.Error.Type});

            proxy.On<ErrorPayload>(
                "error",
                p => _errors.OnNext(p));

            return connection.Start();
        }

        public void Dispose()
        {
            _connection.Dispose();

            _errors.OnCompleted();

            var disposable = _errors as IDisposable;
            if (disposable != null) disposable.Dispose();
        }
    }
}
