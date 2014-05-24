using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RElmah.Common;
using RElmah.Server.Domain;

namespace RElmah.Server.Hubs
{
    [HubName("relmah")]
    public class Frontend : Hub
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IErrorsBacklog _errorsBacklog;

        public Frontend(IConfigurationProvider configurationProvider, IErrorsBacklog errorsBacklog)
        {
            _configurationProvider = configurationProvider;
            _errorsBacklog = errorsBacklog;
        }

        public override Task OnConnected()
        {
            Clients.Caller.clusterOperation(    new Operation<Cluster>(    _configurationProvider.Clusters,     OperationType.Create));

            Clients.Caller.applicationOperation(new Operation<Application>(_configurationProvider.Applications, OperationType.Create));

            return base.OnConnected();
        }

        public Task<IEnumerable<ErrorPayload>> GetErrors()
        {
            return _errorsBacklog.GetErrors();
        }
    }
}
