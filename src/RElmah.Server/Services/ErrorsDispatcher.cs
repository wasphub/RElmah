using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using RElmah.Server.Domain;
using RElmah.Server.Hubs;

namespace RElmah.Server.Services
{
    public class ErrorsDispatcher : IErrorsDispatcher
    {
        private readonly IErrorsInbox _inbox;
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<Frontend>();

        public ErrorsDispatcher(IErrorsInbox inbox)
        {
            _inbox = inbox;
        }

        public Task Dispatch(ErrorDescriptor descriptor)
        {
            return _context.Clients.All.dispatch(descriptor);
        }
    }
}
