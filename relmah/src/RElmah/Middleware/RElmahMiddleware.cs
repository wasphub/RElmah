using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
using RElmah.Extensions;

namespace RElmah.Middleware
{
    public class RElmahMiddleware : OwinMiddleware
    {
        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> _dispatchers;

        public RElmahMiddleware(OwinMiddleware next, IResolver resolver)
            : base(next)
        {
            const string relmah = "relmah";

            var inbox   = new Lazy<IErrorsInbox>(resolver.Resolve<IErrorsInbox>);
            var updater = new Lazy<IConfigurationUpdater>(resolver.Resolve<IConfigurationUpdater>);

            var keyer = new Func<string, string>(s => string.Format("/{0}/{1}", relmah, s));

            _dispatchers = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                { keyer("post-error"),    e => Routes.PostError(inbox.Value, e) },

                { keyer("clusters"),      e => Routes.Clusters(updater.Value, e) },
                { keyer("applications"),  e => Routes.Applications(updater.Value, e) },
                { keyer("users"),         e => Routes.Users(updater.Value, e) },

                { keyer("cluster-users"), e => Routes.ClusterUsers(updater.Value, e) },
            };

        }
         
        public override Task Invoke(IOwinContext context)
        {
            var request  = new OwinRequest(context.Environment);
            var segments = request.Uri.Segments;
            var path     = string.Join(null, segments.Take(2).Concat(segments.Skip(2).Take(1).Select(s => s.Replace("/", ""))));
            var key      = new PathString(path).ToString();

            return _dispatchers.Get(key, e => Next.Invoke(context))(context.Environment);
        }
    }
}