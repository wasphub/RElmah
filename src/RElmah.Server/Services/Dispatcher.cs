using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using RElmah.Common;
using RElmah.Server.Domain;
using RElmah.Server.Hubs;

namespace RElmah.Server.Services
{
    public class Dispatcher : IDispatcher
    {
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<Frontend>();

        public Dispatcher(IErrorsInbox errorsInbox, IConfigurationProvider configurationProvider)
        {
            errorsInbox.GetErrors().Subscribe(p => DispatchError(configurationProvider, p));

            configurationProvider.GetObservableClusters().Subscribe(p => DispatchClusterOperation(configurationProvider, p));
            configurationProvider.GetObservableApplications().Subscribe(p => DispatchApplicationOperation(configurationProvider, p));
        }

        public Task DispatchError(IConfigurationProvider configurationProvider, ErrorPayload payload)
        {
            return _context.Clients.Group(payload.SourceId).error(payload);
        }

        public Task DispatchClusterOperation(IConfigurationProvider configurationProvider, Operation<Cluster> op)
        {
            return _context.Clients.All.clusterOperation(op);
        }

        public Task DispatchApplicationOperation(IConfigurationProvider configurationProvider, Operation<Application> op)
        {
            return _context.Clients.All.applicationOperation(op);
        }
    }
}
