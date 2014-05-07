using System;
using System.Reactive.Subjects;
using Microsoft.AspNet.SignalR.Client;
using RElmah.Domain;

namespace RElmah.Client
{
    public class Connection : IDisposable
    {
        private readonly HubConnection _connection;
        private readonly Subject<ErrorPayload> _subject;

        public Connection(string endpoint)
        {
            _subject = new Subject<ErrorPayload>();

            _connection = new HubConnection(endpoint);

            var proxy = _connection.CreateHubProxy("relmah");
            proxy.On<ErrorPayload>("dispatch", p => _subject.OnNext(p));

            _connection.Start();
        }

        public IObservable<dynamic> Errors { get { return _subject; } } 

        public void Dispose()
        {
            _connection.Dispose();

            _subject.OnCompleted();
            _subject.Dispose();
        }
    }
}
