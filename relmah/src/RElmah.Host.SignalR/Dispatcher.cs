using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using RElmah.Common;
using RElmah.Host.SignalR.Hubs;
using RElmah.Models;
using RElmah.Models.Configuration;

namespace RElmah.Host.SignalR
{
    public class Dispatcher : IDispatcher
    {
        private readonly IConfigurationUpdater  _configurationUpdater;
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>();

        public Dispatcher(IErrorsInbox errorsInbox, IConfigurationProvider configurationProvider, IConfigurationUpdater configurationUpdater)
        {
            _configurationUpdater  = configurationUpdater;

            errorsInbox.GetErrorsStream().Subscribe(p => DispatchError(p));

            configurationProvider.ObserveClusterUsers().Subscribe(DispatchUserApplications);
            configurationProvider.ObserveClusterApplications().Subscribe(DispatchUsersApplication);
        }

        public void DispatchUserApplications(Delta<Relationship<Cluster, User>> payload)
        {
            var apps = 
                from a in payload.Target.Primary.Applications 
                select a.Name;
            apps = apps.ToArray();

            if (payload.Type == DeltaType.Added)
            {
                foreach (var app in apps)
                    foreach (var token in payload.Target.Secondary.Tokens)
                        _context.Groups.Add(token, app);
                
                _context
                    .Clients
                    .User(payload.Target.Secondary.Name)
                    .applications(apps, Enumerable.Empty<string>());
            }
            else
            {
                foreach (var app in apps)
                    foreach (var token in payload.Target.Secondary.Tokens)
                        _context.Groups.Remove(token, app);

                _context
                    .Clients
                    .User(payload.Target.Secondary.Name)
                    .applications(Enumerable.Empty<string>(), apps);
            }
        }

        public void DispatchUsersApplication(Delta<Relationship<Cluster, Application>> payload)
        {
            var apps = 
                from a in payload.Target.Primary.Applications
                select a.Name;
            apps = apps.ToArray();

            var action = payload.Type == DeltaType.Added
                ? new Action<string, string>((t, g) => _context.Groups.Add(t, g))
                : (t, g) => _context.Groups.Remove(t, g);

            foreach (var user in payload.Target.Primary.Users)
            {
                foreach (var token in user.Tokens)
                    action(token, payload.Target.Secondary.Name);

                _context
                    .Clients
                    .User(user.Name)
                    .applications(
                        apps,
                        payload.Type == DeltaType.Removed
                            ? new[] {payload.Target.Secondary.Name}
                            : Enumerable.Empty<string>());
            }
        }

        public Task DispatchError(ErrorPayload payload)
        {
            return _context.Clients.Group(payload.SourceId).error(payload);
        }

        public void Connect(string user, Action<string> connector)
        {
            foreach (var app in _configurationUpdater.GetUserApplications(user))
                connector(app.Name);
        }
    }
}
