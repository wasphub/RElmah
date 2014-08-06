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
            foreach (var app in _configurationProvider.GetVisibleApplications(Context.User))
                Groups.Add(Context.ConnectionId, app.SourceId);

            Clients.Caller.clusterOperation(    
                new Delta<Cluster>(    
                    _configurationProvider.Clusters,     
                    DeltaType.Create));

            Clients.Caller.applicationOperation(
                new Delta<Application>(
                    _configurationProvider.Applications,
                    DeltaType.Create));

            return base.OnConnected();
        }

        public async Task<Delta<ErrorPayload>> GetErrors()
        {
            return new Delta<ErrorPayload>(await _errorsBacklog.GetErrors(), DeltaType.Create);
        }
    }
}
