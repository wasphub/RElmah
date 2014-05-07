using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Domain;

namespace RElmah.Server.Middleware.Handlers
{
    public abstract class PostDispatcher : DispatcherBase
    {
        private readonly IErrorsInbox _inbox;

        protected PostDispatcher(IErrorsInbox inbox)
        {
            _inbox = inbox;
        }

        public async Task ProcessRequest(IDictionary<string, object> environment)
        {
            var payload = await OnProcessRequest(environment);

            _inbox.Post(payload);
        }

        protected abstract Task<ErrorPayload> OnProcessRequest(IDictionary<string, object> environment);
    }
}