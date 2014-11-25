using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin;
using RElmah.Extensions;

namespace RElmah.Middleware
{
    public class RElmahMiddleware : OwinMiddleware
    {
        private readonly IDictionary<string, Func<IDictionary<string, object>, IDictionary<string, string>, Task>> _routes;

        public RElmahMiddleware(OwinMiddleware next, IResolver resolver)
            : base(next)
        {
            const string relmah = "relmah";

            var inbox = new Lazy<IErrorsInbox>(resolver.Resolve<IErrorsInbox>);
            var updater = new Lazy<IConfigurationUpdater>(resolver.Resolve<IConfigurationUpdater>);

            var keyer = new Func<string, string>(s => string.Format("/{0}/{1}", relmah, ToRegex(s)));

            _routes = new Dictionary<string, Func<IDictionary<string, object>, IDictionary<string, string>, Task>>
            {
                { keyer("post-error"),                       (e, ks) => Routes.PostError(inbox.Value, e, ks) },
                
                { keyer("clusters/{cluster}/apps/{app}"),    (e, ks) => Routes.ClusterApplications(updater.Value, e, ks) },
                { keyer("clusters/{cluster}/apps"),          (e, ks) => Routes.ClusterApplications(updater.Value, e, ks) },
                { keyer("clusters/{cluster}/users/{user}"),  (e, ks) => Routes.ClusterUsers(updater.Value, e, ks) },
                { keyer("clusters/{cluster}/users"),         (e, ks) => Routes.ClusterUsers(updater.Value, e, ks) },
                { keyer("clusters/{cluster}"),               (e, ks) => Routes.Clusters(updater.Value, e, ks) },
                { keyer("clusters"),                         (e, ks) => Routes.Clusters(updater.Value, e, ks) },
                
                { keyer("applications/(?'app'.*)"),          (e, ks) => Routes.Applications(updater.Value, e, ks) },
                { keyer("applications"),                     (e, ks) => Routes.Applications(updater.Value, e, ks) },
                
                { keyer("users/(?'user'.*)"),                (e, ks) => Routes.Users(updater.Value, e, ks) },                
                { keyer("users"),                            (e, ks) => Routes.Users(updater.Value, e, ks) },
            };
        }

        static string ToRegex(string pattern)
        {
            return Regex.Replace(pattern, @"\{(?'x'\w*)\}", @"(?'$1'.*)");
        }

        public override Task Invoke(IOwinContext context)
        {
            var request = new OwinRequest(context.Environment);
            var segments = request.Uri.Segments;
            var raw = string.Join(null, segments);

            var matches =
                from kvp in _routes
                let matcher = new Regex(kvp.Key)
                let match = matcher.Match(raw)
                where match.Success
                let route = _routes[kvp.Key]
                let groups = match.Groups
                    .Cast<Group>()
                    .Index()
                select new
                {
                    Params =
                        from g in groups
                        select new KeyValuePair<string, string>(
                            matcher.GroupNameFromNumber(g.Key),
                            g.Value.Value),
                    Route = route
                };

            var invocation = matches.FirstOrDefault();

            return invocation != null 
                 ? invocation.Route(context.Environment, invocation.Params.Where(k => k.Key != "0").ToDictionary(k => k.Key, v => v.Value)) 
                 : Next.Invoke(context);
        }
    }
}