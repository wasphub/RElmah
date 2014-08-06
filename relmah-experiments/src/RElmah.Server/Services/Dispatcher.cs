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
            errorsInbox          .GetErrors()           .Subscribe(p => DispatchError(configurationProvider, p));

            configurationProvider.GetClusterUserDeltas().Subscribe(p => DispatchClusterUserOperation(configurationProvider, p));
        }

        public Task DispatchError(IConfigurationProvider configurationProvider, ErrorPayload payload)
        {
            return _context.Clients.Group(payload.SourceId).error(payload);
        }

        public Task DispatchClusterUserOperation(IConfigurationProvider configurationProvider, Delta<ClusterUser> op)
        {
            return _context.Clients.All.clusterUserOperation(op);
        }
    }
}
