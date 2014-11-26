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
        public delegate Task<object> AsyncHttpFormRequest(IDictionary<string, object> env, IDictionary<string, string> keys, IFormCollection form);

        public class Route
        {
            private readonly ImmutableDictionary<string, AsyncHttpRequest>     _handlers     = ImmutableDictionary<string, AsyncHttpRequest>.Empty;
            private readonly ImmutableDictionary<string, AsyncHttpFormRequest> _formHandlers = ImmutableDictionary<string, AsyncHttpFormRequest>.Empty;

            Route()
            {

            }
            Route(Route r, string verb, AsyncHttpRequest handler)
            {
                _handlers     = r._handlers.Add(verb, handler);
                _formHandlers = r._formHandlers;
            }
            Route(Route r, string verb, AsyncHttpFormRequest handler)
            {
                _handlers     = r._handlers;
                _formHandlers = r._formHandlers.Add(verb, handler);
            }

            public static Route Empty
            {
                get { return new Route(); }
            }

            private Route Handle(string verb, AsyncHttpRequest handler)
            {
                return new Route(this, verb, handler);
            }

            private Route Handle(string verb, AsyncHttpFormRequest handler)
            {
                return new Route(this, verb, handler);
            }

            public Route Get(AsyncHttpRequest handler)
            {
                return Handle("GET", handler);
            }
            public Route Delete(AsyncHttpRequest handler)
            {
                return Handle("DELETE", handler);
            }
            public Route Post(AsyncHttpFormRequest handler)
            {
                return Handle("POST", handler);
            }

            public async Task Invoke(IDictionary<string, object> environment, IDictionary<string, string> keys)
            {
                var request     = new OwinRequest(environment);
                var response    = new OwinResponse(environment);

                var handler     = _handlers    .GetValueOrDefault(request.Method, async (_, __) =>      await Task.FromResult((object)null));
                var formHandler = _formHandlers.GetValueOrDefault(request.Method, async (_, __, ___) => await Task.FromResult((object)null));

                var r           = request.Method == "POST"
                                ? await formHandler(environment, keys, await request.ReadFormAsync()) 
                                : await handler(environment, keys);

                response.StatusCode = r == null ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;

                await response.WriteAsync(JsonConvert.SerializeObject(r));
            }
        }

        public class RouteBuilder
        {
            private readonly ImmutableDictionary<string, Route> _routes = ImmutableDictionary<string, Route>.Empty;
            private readonly ImmutableList<string> _keys = ImmutableList<string>.Empty;
                    
            RouteBuilder()
            {

            }
            RouteBuilder(RouteBuilder rb, string pattern, Route route)
            {
                _routes = rb._routes.Add(pattern, route);
                _keys   = rb._keys.Add(pattern);
            }

            public IDictionary<string, Route> Routes { get { return _routes; } }
            public IEnumerable<string> RoutesKeys { get { return _keys; } }

            public static RouteBuilder Empty
            {
                get { return new RouteBuilder(); }
            }

            public RouteBuilder ForRoute(string pattern, Func<Route, Route> buildRoute)
            {
                return new RouteBuilder(this, pattern, buildRoute(Route.Empty));
            }
        }

        private static RouteBuilder _builder = RouteBuilder.Empty;

        public static void Build(Func<RouteBuilder, RouteBuilder> build)
        {
            _builder = build(_builder);
        }

        static string ToRegex(string pattern)
        {
            return Regex.Replace(pattern, @"\{(?'x'\w*)\}", @"(?'$1'.*)");
        }

        public static Task Invoke(IOwinContext context, Func<IOwinContext, Task> next)
        {
            var request  = new OwinRequest(context.Environment);
            var segments = request.Uri.Segments;
            var raw      = String.Join(null, segments);

            var matches =
                from key in _builder.RoutesKeys
                let matcher = new Regex(ToRegex(key))
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
                    Route  = _builder.Routes[key]
                };

            var invocation = matches.FirstOrDefault();

            return invocation != null
                ? invocation.Route.Invoke(context.Environment, invocation.Params)
                : next(context);
        }
    }
}