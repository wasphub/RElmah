using System;
using System.Reactive.Linq;
using Microsoft.AspNet.SignalR;
using RElmah.Server.Hubs;

namespace RElmah.Server.Services
{
    public class ErrorsDispatcher : IErrorsDispatcher
    {
        private readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<Frontend>();

        public ErrorsDispatcher(IErrorsInbox inbox, IConfigurationProvider configuration)
        {
            var es1 =
                from e in inbox.GetErrors()
                select e;

            es1.Subscribe(p => _context.Clients.Groups(configuration.GetGroups(p)).dispatch(p));
        }
    }
}
