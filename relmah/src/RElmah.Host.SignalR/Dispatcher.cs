using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using RElmah.Host.SignalR.Hubs;
using RElmah.Models.Errors;

namespace RElmah.Host.SignalR
{
    public class Dispatcher : IDispatcher
    {
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<ErrorsHub>();

        public Dispatcher(IErrorsInbox errorsInbox)
        {
            errorsInbox.GetErrorsStream().Subscribe(p => DispatchError(p));
        }

        public Task DispatchError(ErrorPayload payload)
        {
            return _context.Clients.All.error(payload);
        }
    }
}
