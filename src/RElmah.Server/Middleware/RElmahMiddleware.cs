using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using RElmah.Server.Extensions;
using RElmah.Server.Middleware.Handlers;

namespace RElmah.Server.Middleware
{
    public class RElmahMiddleware : OwinMiddleware
    {
        private readonly PathString _relmahPostEndpoint;
        private readonly PathString _relmahRandomEndpoint;

        private readonly IErrorsInbox _inbox;

        private readonly IDictionary<string, Func<IErrorsInbox, IDictionary<string, object>, Task>> _dispatchers;

        public RElmahMiddleware(OwinMiddleware next, Configuration configuration, IDependencyResolver resolver)
            : base(next)
        {
            const string relmah = "relmah";

            _inbox = resolver.Resolve<IErrorsInbox>();

            _relmahPostEndpoint   = new PathString(string.Format("/{0}/post-error",   configuration.Root ?? relmah));
            _relmahRandomEndpoint = new PathString(string.Format("/{0}/random-error", configuration.Root ?? relmah));

            _dispatchers = new Dictionary<string, Func<IErrorsInbox, IDictionary<string, object>, Task>>
            {
                { _relmahPostEndpoint.ToString(),   (i, e) => new ElmahDispatcher(i).ProcessRequest(e) },
                { _relmahRandomEndpoint.ToString(), (i, e) => new RandomDispatcher(i).ProcessRequest(e) },
            };
        }

        public override Task Invoke(IOwinContext context)
        {
            var request = new OwinRequest(context.Environment);
            return _relmahPostEndpoint == new PathString(request.Path.Value)
                 ? _dispatchers.Get(_relmahPostEndpoint.ToString(), (c, i) => Next.Invoke(context))(_inbox, context.Environment)
                 : _relmahRandomEndpoint == new PathString(request.Path.Value)
                   ? _dispatchers.Get(_relmahRandomEndpoint.ToString(), (c, i) => Next.Invoke(context))(_inbox, context.Environment)
                   : Next.Invoke(context);
        }
    }
}