using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Domain;

namespace RElmah.Server.Middleware.Handlers
{
    public abstract class InboxDispatcher
    {
        private readonly IErrorsInbox _inbox;

        protected InboxDispatcher(IErrorsInbox inbox)
        {
            _inbox = inbox;
        }

        public async Task ProcessRequest(IDictionary<string, object> environment)
        {
            _inbox.Post(await OnProcessRequest(environment));
        }

        protected abstract Task<ErrorPayload> OnProcessRequest(IDictionary<string, object> environment);
    }
}