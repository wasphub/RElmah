using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using RElmah.Common;
using RElmah.Client.Models;

namespace RElmah.Client
{
    public class Connection : IDisposable
    {
        private readonly HubConnection _connection;

        private readonly Subject<ErrorPayload> _errors = new Subject<ErrorPayload>();
        private readonly Subject<Delta<Cluster>> _clusters = new Subject<Delta<Cluster>>();
        private readonly Subject<Delta<Application>> _applications = new Subject<Delta<Application>>();

        public Connection(string endpoint)
        {
            _connection = new HubConnection(endpoint);

            var proxy = _connection.CreateHubProxy("relmah");

            proxy.On<ErrorPayload>(
                "error", 
                p => _errors.OnNext(p));

            proxy.On<Delta<Cluster>>(
                "clusterOperation",
                p => _clusters.OnNext(p));

            proxy.On<Delta<Application>>(
                "applicationOperation", 
                p => _applications.OnNext(p));
        }

        public Task Start()
        {
            return _connection.Start();      
        }

        public IObservable<ErrorPayload> Errors { get { return _errors; } }
        public IObservable<Delta<Cluster>> Clusters { get { return _clusters; } }

        public IObservable<Delta<Application>> Applications { get { return _applications; } } 

        public void Dispose()
        {
            _connection.Dispose();

            _errors.OnCompleted();
            _errors.Dispose();
        }
    }
}
