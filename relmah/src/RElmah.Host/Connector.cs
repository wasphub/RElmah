using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.AspNet.SignalR;
using RElmah.Common;
using RElmah.Extensions;
using RElmah.Foundation;
using RElmah.Grounding;
using RElmah.Host.Hubs;

namespace RElmah.Host
{
    public class Connector : IConnector
    {
        private readonly IErrorsInbox _errorsInbox;
        private readonly IDomainReader _domainReader;
        private readonly IDomainWriter  _domainWriter;

        private readonly AtomicImmutableDictionary<string, LayeredDisposable> _subscriptions = new AtomicImmutableDictionary<string, LayeredDisposable>(); 

        readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>();

        public Connector(IErrorsInbox errorsInbox, IDomainReader domainReader, IDomainWriter domainWriter)
        {
            _errorsInbox = errorsInbox;
            _domainReader = domainReader;
            _domainWriter  = domainWriter;
        }

        public void Start()
        {
            //user additions

            var userAdditions =
                from p in _domainReader.ObserveClusterUsers()
                where p.Type == DeltaType.Added
                select p;

            userAdditions.Subscribe(u => _context
                .Clients.User(u.Target.Secondary.Name)
                .applications(
                    from a in u.Target.Primary.Applications
                    select a.Name,
                    Enumerable.Empty<string>()));

            var groupAdditions =
                from p in userAdditions
                from app in p.Target.Primary.Applications
                from token in p.Target.Secondary.Tokens
                select new { token, app = app.Name };
            groupAdditions.Subscribe(p => _context.Groups.Add(p.token, p.app));


            //User removals

            var userRemovals =
                from p in _domainReader.ObserveClusterUsers()
                where p.Type == DeltaType.Removed
                select p;

            userRemovals.Subscribe(u => _context
                .Clients.User(u.Target.Secondary.Name)
                .applications(
                    Enumerable.Empty<string>(),
                    from a in u.Target.Primary.Applications
                    select a.Name));

            var groupRemovals =
                from p in userRemovals
                from app in p.Target.Primary.Applications
                from token in p.Target.Secondary.Tokens
                select new { token, app = app.Name };
            groupRemovals.Subscribe(p => _context.Groups.Remove(p.token, p.app));


            //apps deltas

            var appDeltas =
                from p in _domainReader.ObserveClusterApplications()
                let action = p.Type == DeltaType.Added
                             ? new Action<string, string>((t, g) => _context.Groups.Add(t, g))
                             : (t, g) => _context.Groups.Remove(t, g)
                let target = p.Target.Secondary.Name
                let removals = p.Type == DeltaType.Added
                             ? Enumerable.Empty<string>()
                             : new[] { target }
                from user in p.Target.Primary.Users
                select new
                {
                    p.Type,
                    User = user,
                    Action = action,
                    Additions = from a in p.Target.Primary.Applications
                                select a.Name,
                    Removals = removals,
                    Target = target
                };

            appDeltas.Subscribe(p =>
                p.User.Tokens.Each(t => p.Action(t, p.Target)));

            appDeltas.Subscribe(p =>
                _context
                    .Clients.User(p.User.Name)
                    .applications(p.Additions, p.Removals));
        }

        public async void Connect(string user, string token, Action<string> connector)
        {
            var u = await _domainWriter.AddUserToken(user, token);

            //errors
            if (u.HasValue && u.Value.Tokens.Count() == 1)
            {
                var errors =
                    from e in _errorsInbox.GetErrorsStream()
                    from app in _domainWriter.GetUserApplications(user).ToObservable()
                    where e.SourceId == app.Name
                    select e;

                var d = errors
                    .Subscribe(payload => _context.Clients.User(user).error(payload))
                    .ToLayeredDisposable();

                _subscriptions.SetItem(user, d);
            }
            else
                _subscriptions.Get(user).Wrap();


            //apps
            var apps = _domainWriter.GetUserApplications(user);
            apps.ToObservable().Do(app => connector(app.Name)).Subscribe();
        }

        public async void Disconnect(string token)
        {
            var u = await _domainWriter.RemoveUserToken(token);
            if (!u.HasValue) return;

            var name         = u.Value.Name;
            var subscription = _subscriptions.Get(name);

            subscription.Dispose();

            if (subscription.IsDisposed)
                _subscriptions.Remove(name);
        }       
    }
}