using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using RElmah.Host.SignalR.Hubs;
using RElmah.Models;
using RElmah.Models.Configuration;
using RElmah.Models.Errors;

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
            _context
                .Clients
                .User(payload.Target.Secondary.Name)
                .applications(from a in payload.Target.Primary.Applications select a.Name);
        }

        public void DispatchUsersApplication(Delta<Relationship<Cluster, Application>> payload)
        {
            foreach (var user in payload.Target.Primary.Users)
                _context
                    .Clients
                    .User(user.Name)
                    .applications(from a in payload.Target.Primary.Applications select a.Name);
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
