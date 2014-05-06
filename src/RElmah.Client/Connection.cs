using System;
using System.Reactive.Subjects;
using Microsoft.AspNet.SignalR.Client;

namespace RElmah.Client
{
    public class Connection : IDisposable
    {
        private readonly HubConnection _connection;
        private readonly Subject<int> _subject;

        public Connection(string endpoint)
        {
            _subject = new Subject<int>();

            _connection = new HubConnection(endpoint);

            var random = new Random();
            var proxy = _connection.CreateHubProxy("relmah");
            proxy.On<dynamic>("dispatch", p => _subject.OnNext(random.Next(100)));

            _connection.Start();
        }

        public IObservable<int> Errors { get { return _subject; } } 

        public void Dispose()
        {
            _connection.Dispose();

            _subject.OnCompleted();
            _subject.Dispose();
        }
    }
}
