using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using RElmah.Middleware;

namespace RElmah.Host.SignalR
{
    public class RElmahMiddleware : OwinMiddleware
    {
        private readonly Lazy<IErrorsInbox> _inbox;

        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> _dispatchers;

        public RElmahMiddleware(OwinMiddleware next, IDependencyResolver resolver)
            : base(next)
        {
            const string relmah = "relmah";

            _inbox = new Lazy<IErrorsInbox>(resolver.Resolve<IErrorsInbox>);

            var keyer = new Func<string, string>(s => string.Format("/{0}/{1}", relmah, s));

            _dispatchers = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                { keyer("post-error"),   e => Routes.PostError(_inbox.Value, Routes.Elmah, e) }
            };

        }

        public override Task Invoke(IOwinContext context)
        {
            return Next.Invoke(context);
        }
    }
}