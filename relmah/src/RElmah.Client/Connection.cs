using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using RElmah.Common;
using RElmah.Extensions;

namespace RElmah.Client
{
    public class Connection : IDisposable
    {
        private readonly string _endpoint;

        private HubConnection _connection;

        private readonly ISubject<ErrorPayload>         _errors = new Subject<ErrorPayload>();
        private readonly ISubject<SourceOperation> _sources = new Subject<SourceOperation>();
        private readonly ISubject<RecapAggregate>       _recaps = new Subject<RecapAggregate>();

        public Connection(string endpoint)
        {
            _endpoint = endpoint;
        }

        public IObservable<ErrorPayload> Errors { get { return _errors; } }
        public IObservable<IGroupedObservable<ErrorType, ErrorPayload>> ErrorTypes { get; private set; }

        public IObservable<SourceOperation> Sources { get { return _sources; } }

        public IObservable<RecapAggregate> Recaps { get { return _recaps; } }

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

        public enum SourceOperationType
        {
            Added,
            Removed
        }

        public class SourceOperation
        {
            public readonly string SourceId;
            public readonly SourceOperationType Type;

            public SourceOperation(string sourceId, SourceOperationType type)
            {
                SourceId = sourceId;
                Type     = type;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((SourceId != null ? SourceId.GetHashCode() : 0) * 397) ^ (Type.GetHashCode());
                }
            }

            public bool Equals(SourceOperation target)
            {
                if (target == null) return false;
                return string.Equals(SourceId, target.SourceId) && Type == target.Type;
            }
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is SourceOperation && Equals((SourceOperation)obj);
            }
        }

        public class RecapAggregate
        {
            public readonly string Name;
            public readonly string Type;
            public readonly int Measure;

            public RecapAggregate(string name, string type, int measure)
            {
                Name    = name;
                Type    = type;
                Measure = measure;
            }

            protected bool Equals(RecapAggregate other)
            {
                return string.Equals(Name, other.Name) && string.Equals(Type, other.Type) && Measure == other.Measure;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (Name != null ? Name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Measure;
                    return hashCode;
                }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((RecapAggregate) obj);
            }
        }

        private Task Connect(HubConnection connection)
        {
            var errorsProxy = connection.CreateHubProxy("relmah-errors");

            //streams by error type
            ErrorTypes = _errors.GroupBy(e => new ErrorType(e.SourceId, e.Error.Type));

            //errors
            errorsProxy.On<ErrorPayload>(
                "error",
                p => _errors.OnNext(p));

            //sources visibility
            var sources = new HashSet<string>();
            errorsProxy.On<IEnumerable<string>, IEnumerable<string>>(
                "sources",
                (es, rs) =>
                {
                    foreach (var e in es.Where(e => !sources.Contains(e)))
                    {
                        _sources.OnNext(new SourceOperation(e,  SourceOperationType.Added));
                        sources.Add(e);
                    }
                    foreach (var r in rs.Where(e => sources.Contains(e)))
                    {
                        _sources.OnNext(new SourceOperation(r, SourceOperationType.Removed));
                        sources.Remove(r);
                    } 
                });

            //recaps
            var groups = new Dictionary<string, IDisposable>();
            errorsProxy.On<Recap>(
                "recap",
                p =>
                {
                    groups["*"] = ErrorTypes.Subscribe(et =>
                    {
                        var key = et.Key.SourceId + '-' + et.Key.Type;
                        groups.Do(key, d => d.Dispose());

                        var rs =
                            from a in p.Sources
                            where a.SourceId == et.Key.SourceId
                            from b in a.Types
                            where b.Name == et.Key.Type
                            select b.Measure;

                        var r = rs.Aggregate(0, (acc, cur) => acc + cur);

                        groups[key] = et
                            .Scan(0, (ka, ep) => ka + 1)
                            .Subscribe(e =>
                            {
                                _recaps.OnNext(new RecapAggregate(et.Key.SourceId, et.Key.Type, e + r));
                            });
                    });
                });

            return connection.Start();
        }

        public void Dispose()
        {
            _connection.Dispose();

            _errors.OnCompleted();
            _sources.OnCompleted();
            _recaps.OnCompleted();

            var disposable = _errors as IDisposable;
            if (disposable != null) disposable.Dispose();
            disposable = _sources as IDisposable;
            if (disposable != null) disposable.Dispose();
            disposable = _recaps as IDisposable;
            if (disposable != null) disposable.Dispose();
        }
    }
}
