using System;
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
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>();

        public Dispatcher(IErrorsInbox errorsInbox, IConfigurationProvider configurationProvider)
        {
            errorsInbox.GetErrorsStream().Subscribe(p => DispatchError(p));

            configurationProvider.ObserveClusterUsers().Subscribe(p => DispatchUserApplications(p));
        }

        public Task DispatchUserApplications(Delta<Relationship<Cluster, User>> payload)
        {
            return _context
                .Clients
                .User(payload.Target.Secondary.Name)
                .applications(payload.Target.Primary.Applications);
        }

        public Task DispatchError(ErrorPayload payload)
        {
            return _context.Clients.All.error(payload);
        }
    }
}
