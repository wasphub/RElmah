using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using RElmah.Extensions;
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
            var request = new OwinRequest(context.Environment);
            var segments = request.Uri.Segments;
            var path = string.Join(null, segments.Take(2).Concat(segments.Skip(2).Take(1).Select(s => s.Replace("/", ""))));
            var key = new PathString(path).ToString();

            return _dispatchers.ContainsKey(key)
                 ? _dispatchers.Get(key, e => Next.Invoke(context))(context.Environment)
                 : Next.Invoke(context);
        }
    }
}