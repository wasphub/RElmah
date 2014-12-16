using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using RElmah.Extensions;

namespace RElmah.Middleware
{
    internal static class Router
    {
        public delegate Task<object> AsyncHttpRequest    (IDictionary<string, object> env, IDictionary<string, string> keys);
        public delegate Task<object> AsyncHttpFormRequest(IDictionary<string, object> env, IDictionary<string, string> keys, IDictionary<string, string> form);

        public class Route
        {
            private readonly ImmutableDictionary<string, AsyncHttpRequest>     _handlers     = ImmutableDictionary<string, AsyncHttpRequest>.Empty;
            private readonly ImmutableDictionary<string, AsyncHttpFormRequest> _formHandlers = ImmutableDictionary<string, AsyncHttpFormRequest>.Empty;

            private readonly Func<object, int> _statusCodeGenerator = null;

            Route()
            {

            }
            Route(Route r, string verb, AsyncHttpRequest handler, Func<object, int> statusCodeGenerator)
            {
                _statusCodeGenerator = statusCodeGenerator;
                _handlers            = r._handlers.Add(verb, handler);
                _formHandlers        = r._formHandlers;
            }
            Route(Route r, string verb, AsyncHttpFormRequest handler, Func<object, int> statusCodeGenerator)
            {
                _statusCodeGenerator = statusCodeGenerator;
                _handlers            = r._handlers;
                _formHandlers        = r._formHandlers.Add(verb, handler);
            }

            public static Route Empty
            {
                get { return new Route(); }
            }

            private Route Handle(string verb, AsyncHttpRequest handler, Func<object, int> statusCodeGenerator = null)
            {
                return new Route(this, verb, handler, statusCodeGenerator);
            }

            private Route Handle(string verb, AsyncHttpFormRequest handler, Func<object, int> statusCodeGenerator = null)
            {
                return new Route(this, verb, handler, statusCodeGenerator);
            }

            public Route Get(AsyncHttpRequest handler, Func<object, int> statusCodeGenerator = null)
            {
                return Handle("GET",
                    handler,
                    statusCodeGenerator ?? (r => 
                        r == null
                        ? (int)HttpStatusCode.NotFound
                        : (int)HttpStatusCode.OK));
            }
            public Route Delete(AsyncHttpRequest handler, Func<object, int> statusCodeGenerator = null)
            {
                return Handle("DELETE", 
                    handler,
                    statusCodeGenerator ?? (r =>
                        r == null
                        ? (int)HttpStatusCode.NoContent
                        : (int)HttpStatusCode.OK));
            }

            public Route Put(AsyncHttpFormRequest handler, Func<object, int> statusCodeGenerator = null)
            {
                return Handle("PUT",
                    handler,
                    statusCodeGenerator ?? (r =>
                        r == null
                        ? (int)HttpStatusCode.NoContent
                        : (int)HttpStatusCode.OK));
            }
            
            public Route Post(AsyncHttpFormRequest handler, Func<object, int> statusCodeGenerator = null)
            {
                return Handle("POST",
                    handler,
                    statusCodeGenerator ?? (r =>
                        r == null
                        ? (int)HttpStatusCode.NoContent
                        : (int)HttpStatusCode.Created));
            }

            public async Task Invoke(IDictionary<string, object> environment, IDictionary<string, string> keys)
            {
                var request     = new OwinRequest(environment);

                var handler     = _handlers    .GetValueOrDefault(request.Method, async (_, __) =>      await Task.FromResult((object)null));
                var formHandler = _formHandlers.GetValueOrDefault(request.Method, async (_, __, ___) => await Task.FromResult((object)null));

                keys            = keys.ToDictionary(k => k.Key, v => WebUtility.UrlDecode(v.Value));

                var rf          = request.HasForm()
                                ? (Func<Task<object>>)(async () => formHandler(
                                    environment, 
                                    keys, 
                                    from w in (await request.ReadFormAsync()) 
                                    select w != null ? w[0] : null))
                                : () => handler(
                                    environment, 
                                    keys);

                var r           = await rf();

                var response = new OwinResponse(environment)
                {
                    StatusCode = _statusCodeGenerator != null 
                               ? _statusCodeGenerator(r) 
                               : (int)HttpStatusCode.OK
                };

                await response.WriteAsync(JsonConvert.SerializeObject(r));
            }
        }

        public class RouteBuilder
        {
            private const string DefaultPrefix = "relmah";

            private readonly ImmutableDictionary<string, Route> _routes = ImmutableDictionary<string, Route>.Empty;
            private readonly ImmutableList<string> _keys = ImmutableList<string>.Empty;

            public string Prefix { get; private set; }
        
            RouteBuilder(string prefix)
            {
                Prefix = String.IsNullOrWhiteSpace(prefix) ? DefaultPrefix : prefix;
            }
            RouteBuilder(RouteBuilder rb, string pattern, Route route)
            {
                Prefix  = rb.Prefix;

                _routes = rb._routes.Add(pattern, route);
                _keys   = rb._keys.Add(pattern);
            }

            public IDictionary<string, Route> Routes { get { return _routes; } }
            public IEnumerable<string> RoutesKeys { get { return _keys; } }

            public static RouteBuilder Empty
            {
                get { return new RouteBuilder(DefaultPrefix); }
            }

            public RouteBuilder WithPrefix(string prefix)
            {
                return new RouteBuilder(prefix);
            }

            public RouteBuilder ForRoute(string pattern, Func<Route, Route> buildRoute)
            {
                return new RouteBuilder(this, pattern, buildRoute(Route.Empty));
            }
        }

        private static ImmutableList<RouteBuilder> _builders = ImmutableList<RouteBuilder>.Empty;

        public static void Build(Func<RouteBuilder, RouteBuilder> build)
        {
            _builders = _builders.Add(build(RouteBuilder.Empty));
        }

        static string ToRegex(string pattern)
        {
            return Regex.Replace(pattern, @"\{(?'x'\w*)\}", @"(?'$1'[^/]+)");
        }

        public static Task Invoke(IOwinContext context, Func<IOwinContext, Task> next)
        {
            var request  = new OwinRequest(context.Environment);
            var segments = request.Uri.Segments;
            var raw      = String.Join(null, segments);

            var matches =
                from builder in _builders
                from key in builder.RoutesKeys
                let matcher = new Regex(String.Format("^/{0}/{1}/?$", builder.Prefix, ToRegex(key)))
                let match   = matcher.Match(raw)
                where match.Success
                let groups  = match
                    .Groups
                    .Cast<Group>()
                    .Index()
                let @params = (from g in groups
                    select new KeyValuePair<string, string>(
                        matcher.GroupNameFromNumber(g.Key),
                        g.Value.Value))
                select new
                {
                    Params = @params.Skip(1).ToDictionary(k => k.Key, v => v.Value),
                    Route  = builder.Routes[key]
                };

            var invocation = matches.FirstOrDefault();

            return invocation != null
                ? invocation.Route.Invoke(context.Environment, invocation.Params)
                : next(context);
        }
    }
}