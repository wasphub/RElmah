using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using RElmah.Server.Extensions;
using RElmah.Server.Middleware.Handlers;

namespace RElmah.Server.Middleware
{
    public class RElmahMiddleware : OwinMiddleware
    {
        private readonly IErrorsInbox _inbox;
        private readonly IConfigurationProvider _config;

        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> _dispatchers;

        public RElmahMiddleware(OwinMiddleware next, Configuration configuration, IDependencyResolver resolver)
            : base(next)
        {
            const string relmah = "relmah";

            var errorsInbox = resolver.Resolve<IErrorsInbox>();
            _inbox  = errorsInbox;
            _config = resolver.Resolve<IConfigurationProvider>();

            var keyer = new Func<string, string>(s => string.Format("/{0}/{1}", configuration.Root ?? relmah, s));

            _dispatchers = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                { keyer("post-error"),   e => Dispatchers.PostError(_inbox,      Dispatchers.Elmah,        e) },
                { keyer("random-error"), e => Dispatchers.PostError(_inbox,      Dispatchers.Random,       e) },

                { keyer("clusters"),     e => Dispatchers.Configuration(_config, e) },
            };
        }

        public override Task Invoke(IOwinContext context)
        {
            var request  = new OwinRequest(context.Environment);
            var segments = request.Uri.Segments;
            var path     = string.Join(null, segments.Take(2).Concat(segments.Skip(2).Take(1).Select(s => s.Replace("/", ""))));
            var key      = new PathString(path).ToString();

            return _dispatchers.ContainsKey(key)
                 ? _dispatchers.Get(key, e => Next.Invoke(context))(context.Environment)
                 : Next.Invoke(context);
        }
    }
}