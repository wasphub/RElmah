using System;
using System.Reactive.Subjects;
using Microsoft.AspNet.SignalR.Client;
using RElmah.Domain;

namespace RElmah.Client
{
    public class Connection : IDisposable
    {
        private readonly HubConnection _connection;

        private readonly Subject<ErrorPayload> _errors = new Subject<ErrorPayload>();
        private readonly Subject<Operation<Cluster>> _clusters = new Subject<Operation<Cluster>>();
        private readonly Subject<Operation<Application>> _applications = new Subject<Operation<Application>>();

        public Connection(string endpoint)
        {
            _connection = new HubConnection(endpoint);

            var proxy = _connection.CreateHubProxy("relmah");

            proxy.On<ErrorPayload>(
                "error", 
                p => _errors.OnNext(p));

            proxy.On<Operation<Cluster>>(
                "clusterOperation",
                p => _clusters.OnNext(p));

            proxy.On<Operation<Application>>(
                "applicationOperation", 
                p => _applications.OnNext(p));

            _connection.Start();
        }

        public IObservable<ErrorPayload> Errors { get { return _errors; } }
        public IObservable<Operation<Cluster>> Clusters { get { return _clusters; } } 

        public void Dispose()
        {
            _connection.Dispose();

            _errors.OnCompleted();
            _errors.Dispose();
        }
    }
}
