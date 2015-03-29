using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly ISubject<ErrorPayload> _errors = new Subject<ErrorPayload>();
        private readonly ISubject<ApplicationOperation> _applications = new Subject<ApplicationOperation>();

        public Connection(string endpoint)
        {
            _endpoint = endpoint;
        }

        public IObservable<ErrorPayload> Errors { get { return _errors; } }
        public IObservable<IGroupedObservable<ErrorType, ErrorPayload>> ErrorTypes { get; private set; }

        public IObservable<ApplicationOperation> Applications { get { return _applications; } }

        public Task Start(ClientToken token)
        {
            _connection = new HubConnection(_endpoint, string.Format("user={0}", token.Token));

            return Connect(_connection);
        }

        public Task Start(ICredentials credentials)
        {
            _connection = new HubConnection(_endpoint) { Credentials = credentials };

            return Connect(_connection);
        }

        public Task Start()
        {
            return Start(CredentialCache.DefaultCredentials);
        }

        public class ErrorType
        {
            public readonly string SourceId;
            public readonly string Type;

            public ErrorType(string sourceId, string type)
            {
                SourceId = sourceId;
                Type = type;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((SourceId != null ? SourceId.GetHashCode() : 0)*397) ^ (Type != null ? Type.GetHashCode() : 0);
                }
            }

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

        public enum ApplicationOperationType
        {
            Added,
            Removed
        }

        public class ApplicationOperation
        {
            public readonly string Name;
            public readonly ApplicationOperationType Type;

            public ApplicationOperation(string name, ApplicationOperationType type)
            {
                Name = name;
                Type = type;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Type.GetHashCode());
                }
            }

            public bool Equals(ApplicationOperation target)
            {
                if (target == null) return false;
                return string.Equals(Name, target.Name) && Type == target.Type;
            }
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is ApplicationOperation && Equals((ApplicationOperation)obj);
            }
        }

        private Task Connect(HubConnection connection)
        {
            var errorsProxy = connection.CreateHubProxy("relmah-errors");

            ErrorTypes = _errors.GroupBy(e => new ErrorType(e.SourceId, e.Error.Type));

            errorsProxy.On<ErrorPayload>(
                "error",
                p => _errors.OnNext(p));

            var apps = new HashSet<string>();

            errorsProxy.On<IEnumerable<string>, IEnumerable<string>>(
                "applications",
                (es, rs) =>
                {
                    foreach (var e in es.Where(e => !apps.Contains(e)))
                    {
                        _applications.OnNext(new ApplicationOperation(e,  ApplicationOperationType.Added));
                        apps.Add(e);
                    }
                    foreach (var r in rs.Where(e => apps.Contains(e)))
                    {
                        _applications.OnNext(new ApplicationOperation(r, ApplicationOperationType.Removed));
                        apps.Remove(r);
                    } 
                });

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
