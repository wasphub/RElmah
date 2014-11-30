using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.AspNet.SignalR;
using RElmah.Common;
using RElmah.Extensions;
using RElmah.Host.Hubs;

namespace RElmah.Host
{
    public static class Dispatcher
    {
        public static void Wire(IErrorsInbox errorsInbox, IConfigurationProvider configurationProvider)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>();

            //errors

            errorsInbox
                .GetErrorsStream()
                .Subscribe(payload => context.Clients.Group(payload.SourceId).error(payload));

            #region Complex example
            /*
            errorsInbox
                .GetErrorsStream()
                .Zip(errorsInbox.GetErrorsStream().Skip(1), (a, b) => new {a, b})
                .Where(z => z.a.Error.Type != z.b.Error.Type && z.a.SourceId != z.b.SourceId)
                .Throttle(TimeSpan.FromMilliseconds(3000))
                .Select(z => z.b).Subscribe(payload => context.Clients.Group(payload.SourceId).error(payload));
            */
            #endregion

            //user additions

            var userAdditions = 
                from p in configurationProvider.ObserveClusterUsers()
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
                from p in configurationProvider.ObserveClusterUsers()
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
                from p in configurationProvider.ObserveClusterApplications()
                let action   = p.Type == DeltaType.Added
                             ? new Action<string, string>((t, g) => context.Groups.Add(t, g))
                             : (t, g) => context.Groups.Remove(t, g)
                let target   = p.Target.Secondary.Name
                let removals = p.Type == DeltaType.Added
                             ? Enumerable.Empty<string>()
                             : new[] { target }
                from user in p.Target.Primary.Users
                select new
                {
                    p.Type,
                    User     = user, 
                    Action   = action, 
                    Apps     = p.Target.Primary.Applications,
                    Removals = removals,
                    Target   = target
                };

            appDeltas.Subscribe(p =>
                p.User.Tokens.Each(t => p.Action(t, p.Target)));

            appDeltas.Subscribe(p =>
                context
                    .Clients.User(p.User.Name)
                    .applications(p.Apps, p.Removals));


            #region Trivial way to subscribe
            /*
            errorsInbox.GetErrorsStream().Subscribe(payload => context.Clients.Group(payload.SourceId).error(payload));

            configurationProvider.ObserveClusterUsers().Subscribe(payload =>
            {
                var apps = 
                    from a in payload.Target.Primary.Applications 
                    select a.Name;
                apps = apps.ToArray();

                if (payload.Type == DeltaType.Added)
                {
                    foreach (var token in payload.Target.Secondary.Tokens)
                        foreach (var app in apps)
                            context.Groups.Add(token, app);
                
                    context
                        .Clients
                        .User(payload.Target.Secondary.Name)
                        .applications(apps, Enumerable.Empty<string>());
                }
                else
                {
                    foreach (var app in apps)
                        foreach (var token in payload.Target.Secondary.Tokens)
                            context.Groups.Remove(token, app);

                    context
                        .Clients
                        .User(payload.Target.Secondary.Name)
                        .applications(Enumerable.Empty<string>(), apps);
                }
            });

            configurationProvider.ObserveClusterApplications().Subscribe(payload1 =>
            {
                var apps = 
                    from a in payload1.Target.Primary.Applications
                    select a.Name;
                apps = apps.ToArray();

                var action = payload1.Type == DeltaType.Added
                    ? new Action<string, string>((t, g) => context.Groups.Add(t, g))
                    : (t, g) => context.Groups.Remove(t, g);

                foreach (var user in payload1.Target.Primary.Users)
                {
                    foreach (var token in user.Tokens)
                        action(token, payload1.Target.Secondary.Name);

                    context
                        .Clients
                        .User(user.Name)
                        .applications(
                            apps,
                            payload1.Type == DeltaType.Removed
                                ? new[] {payload1.Target.Secondary.Name}
                                : Enumerable.Empty<string>());
                }
            });
            */
            #endregion
        }
    }
}
