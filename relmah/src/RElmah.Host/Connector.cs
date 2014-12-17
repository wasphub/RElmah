using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.AspNet.SignalR;
using RElmah.Common;
using RElmah.Extensions;
using RElmah.Host.Hubs;

namespace RElmah.Host
{
    public class Connector : IConnector
    {
        private readonly IErrorsInbox _errorsInbox;
        private readonly IDomainReader _domainReader;
        private readonly IDomainWriter  _domainWriter;

        public Connector(IErrorsInbox errorsInbox, IDomainReader domainReader, IDomainWriter domainWriter)
        {
            _errorsInbox = errorsInbox;
            _domainReader = domainReader;
            _domainWriter  = domainWriter;
        }

        public void Start()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>();

            //errors

            _errorsInbox
                .GetErrorsStream()
                .Subscribe(payload => context.Clients.Group(payload.SourceId).error(payload));

            //user additions

            var userAdditions =
                from p in _domainReader.ObserveClusterUsers()
                where p.Type == DeltaType.Added
                select p;

            userAdditions.Subscribe(u => context
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
            groupAdditions.Subscribe(p => context.Groups.Add(p.token, p.app));


            //User removals

            var userRemovals =
                from p in _domainReader.ObserveClusterUsers()
                where p.Type == DeltaType.Removed
                select p;

            userRemovals.Subscribe(u => context
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
            groupRemovals.Subscribe(p => context.Groups.Remove(p.token, p.app));


            //apps deltas

            var appDeltas =
                from p in _domainReader.ObserveClusterApplications()
                let action = p.Type == DeltaType.Added
                             ? new Action<string, string>((t, g) => context.Groups.Add(t, g))
                             : (t, g) => context.Groups.Remove(t, g)
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
                context
                    .Clients.User(p.User.Name)
                    .applications(p.Additions, p.Removals));
        }

        public void Connect(string user, string token, Action<string> connector)
        {
            _domainWriter.AddUserToken(user, token);

            foreach (var app in _domainWriter.GetUserApplications(user))
                connector(app.Name);
        }
    }
}