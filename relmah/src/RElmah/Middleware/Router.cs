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
        public delegate Task<object> AsyncHttpRequest(IDictionary<string, object> env, IDictionary<string, string> keys, IDictionary<string, string> form);

        public class Route
        {
            class AsyncHttpRequestHandler
            {
                public AsyncHttpRequest Executor { get; private set; }
                public Func<object, int> StatusCodeGenerator { get; private set; }
                public AsyncHttpRequestHandler(AsyncHttpRequest executor, Func<object, int> statusCodeGenerator = null)
                {
                    Executor = executor;
                    StatusCodeGenerator = statusCodeGenerator ?? (_ => (int)HttpStatusCode.OK);
                }

                public static readonly AsyncHttpRequestHandler Null = new AsyncHttpRequestHandler(async (_, __, ___) => await Task.FromResult((object) null));
            }

            private readonly ImmutableDictionary<string, AsyncHttpRequestHandler> _handlers = ImmutableDictionary<string, AsyncHttpRequestHandler>.Empty;

            Route()
            {
            }

            Route(Route r, string verb, AsyncHttpRequest handler, Func<object, int> statusCodeGenerator)
            {
                _handlers  = r._handlers.Add(verb, new AsyncHttpRequestHandler(handler, statusCodeGenerator));
            }

            public static Route Empty
            {
                get { return new Route(); }
            }

            private Route Handle(string verb, AsyncHttpRequest handler, Func<object, int> statusCodeGenerator = null)
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

            public Route Put(AsyncHttpRequest handler, Func<object, int> statusCodeGenerator = null)
            {
                return Handle("PUT",
                    handler,
                    statusCodeGenerator ?? (r =>
                        r == null
                        ? (int)HttpStatusCode.NoContent
                        : (int)HttpStatusCode.OK));
            }
            
            public Route Post(AsyncHttpRequest handler, Func<object, int> statusCodeGenerator = null)
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
                var completed = new TaskCompletionSource<bool>();

                try
                {          
                    var request  = new OwinRequest(environment);

                    var handler  = _handlers.GetValueOrDefault(request.Method, AsyncHttpRequestHandler.Null);

                    keys         = keys.ToDictionary(k => k.Key, v => WebUtility.UrlDecode(v.Value));

                    await ExecuteAsync(environment, keys, request, handler).ConfigureAwait(false);

                    completed.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    completed.TrySetException(ex);

                    throw;
                }
            }

            static readonly IDictionary<string, string[]> EmptyForm = new Dictionary<string, string[]>();

            private static async Task ExecuteAsync(
                IDictionary<string, object> environment, 
                IDictionary<string, string> keys, OwinRequest request,
                AsyncHttpRequestHandler handler)
            {
                var response = new OwinResponse(environment);

                var executor = (Func<Task<object>>)(async () =>
                {
                    var form = from w in request.HasForm()
                                         ? (IEnumerable<KeyValuePair<string, string[]>>)(await request.ReadFormAsync().ConfigureAwait(false))
                                         : EmptyForm
                               select w != null ? w[0] : null;

                    return handler.Executor(
                        environment,
                        keys,
                        form.ToDictionary());
                });

                var result = await executor().ConfigureAwait(false);

                await response.WriteAsync(JsonConvert.SerializeObject(result)).ConfigureAwait(false);

                response.StatusCode = handler.StatusCodeGenerator != null
                                    ? handler.StatusCodeGenerator(result)
                                    : (int)HttpStatusCode.OK;
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