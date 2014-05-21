using System;
using System.Reactive.Subjects;
using Microsoft.AspNet.SignalR.Client;
using RElmah.Domain;

namespace RElmah.Client
{
    public class Connection : IDisposable
    {
        private readonly HubConnection _connection;
        private readonly Subject<ErrorPayload> _errors;
        private readonly Subject<UpdateEntry<Cluster>> _clusters;

        public Connection(string endpoint)
        {
            _errors = new Subject<ErrorPayload>();
            _clusters = new Subject<UpdateEntry<Cluster>>();

            _connection = new HubConnection(endpoint);

            var proxy = _connection.CreateHubProxy("relmah");

            proxy.On<ErrorPayload>(
                "error", 
                p => _errors.OnNext(p));

            proxy.On<UpdateEntry<Cluster>>(
                "clusterUpdate", 
                p => _clusters.OnNext(p));

            _connection.Start();
        }

        public IObservable<ErrorPayload> Errors { get { return _errors; } }
        public IObservable<UpdateEntry<Cluster>> Clusters { get { return _clusters; } } 

        public void Dispose()
        {
            _connection.Dispose();

            _errors.OnCompleted();
            _errors.Dispose();
        }
    }
}
