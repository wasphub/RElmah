using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using RElmah.Common;
using RElmah.Host.Hubs;

namespace RElmah.Host
{
    public static class Dispatcher
    {
        public static void Wire(IErrorsInbox errorsInbox, IConfigurationProvider configurationProvider)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>();

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
        }
    }
}
