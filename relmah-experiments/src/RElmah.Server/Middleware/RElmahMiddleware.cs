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
        private readonly Lazy<IErrorsInbox> _inbox;
        private readonly Lazy<IConfigurationProvider> _config;

        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> _dispatchers;

        public RElmahMiddleware(OwinMiddleware next, Configuration configuration, IDependencyResolver resolver)
            : base(next)
        {
            const string relmah = "relmah";

            _inbox  = new Lazy<IErrorsInbox>(resolver.Resolve<IErrorsInbox>);
            _config = new Lazy<IConfigurationProvider>(resolver.Resolve<IConfigurationProvider>);

            var keyer = new Func<string, string>(s => string.Format("/{0}/{1}", configuration.Root ?? relmah, s));

            _dispatchers = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                { keyer("post-error"),   e => Routes.PostError(_inbox.Value, Routes.Elmah, _config.Value, e) },

                { keyer("clusters"),     e => Routes.Clusters(    _config.Value, e) },
                { keyer("applications"), e => Routes.Applications(_config.Value, e) },
                { keyer("users"),        e => Routes.Users(       _config.Value, e) },
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