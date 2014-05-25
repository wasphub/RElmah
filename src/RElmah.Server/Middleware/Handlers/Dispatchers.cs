using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using RElmah.Common;
using RElmah.Server.Domain;

namespace RElmah.Server.Middleware.Handlers
{
    public static class Dispatchers
    {
        public async static Task PostError(
            IErrorsInbox inbox,
            Func<IConfigurationProvider, IDictionary<string, object>, Task<ErrorPayload>> executor,
            IConfigurationProvider configurationProvider,
            IDictionary<string, object> environment)
        {
            inbox.Post(await executor(configurationProvider, environment));
        }

        public async static Task<ErrorPayload> Elmah(IConfigurationProvider configuration, IDictionary<string, object> environment)
        {
            var @params = await new OwinContext(environment).Request.ReadFormAsync();

            var errorText = Decode(@params["error"]);
            var sourceId  = @params["sourceId"];
            var errorId   = @params["errorId"];
            var infoUrl   = @params["infoUrl"];

            return new ErrorPayload(sourceId, JsonConvert.DeserializeObject<ErrorDetail>(errorText), errorId, infoUrl);
        }

        public async static Task Clusters(
            IConfigurationProvider configuration,
            IDictionary<string, object> environment)
        {
            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                configuration.AddCluster(await BuildCluster(request));
                return;
            }

            var response = new OwinResponse(environment);
            await response.WriteAsync(JsonConvert.SerializeObject(
                request.Uri.Segments.Count() > 3
                ? (dynamic)configuration.GetCluster(request.Uri.Segments.Skip(3).First())
                : configuration.Clusters));
        }

        public async static Task Applications(
            IConfigurationProvider configuration,
            IDictionary<string, object> environment)
        {
            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                var app = await BuildApplication(request, (n, s, c) => new { n, s, c } );
                configuration.AddApplication(app.n, app.s, app.c);
                return;
            }

            var response = new OwinResponse(environment);
            await response.WriteAsync(JsonConvert.SerializeObject(
                request.Uri.Segments.Count() > 3
                ? (dynamic)configuration.GetApplication(request.Uri.Segments.Skip(3).First())
                : configuration.Applications));
        }

        public async static Task Users(
            IConfigurationProvider configuration,
            IDictionary<string, object> environment)
        {
            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                var app = await BuildUser(request, (u, c) => new { u, c });
                configuration.AddUserToCluster(app.u, app.c);
                return;
            }

            var response = new OwinResponse(environment);
            await response.WriteAsync(JsonConvert.SerializeObject(
                request.Uri.Segments.Count() > 3
                ? (dynamic)configuration.GetApplication(request.Uri.Segments.Skip(3).First())
                : configuration.Applications));
        }

        static async Task<string> BuildCluster(OwinRequest request)
        {
            var @params = await request.ReadFormAsync();
            return await Task.FromResult(@params["name"]);
        }

        static async Task<T> BuildApplication<T>(OwinRequest request, Func<string, string, string, T> resultor)
        {
            var @params = await request.ReadFormAsync();
            return await Task.FromResult(resultor(@params["name"], @params["sourceId"], @params["cluster"]));
        }

        static async Task<T> BuildUser<T>(OwinRequest request, Func<string, string, T> resultor)
        {
            var @params = await request.ReadFormAsync();
            return await Task.FromResult(resultor(@params["name"], @params["cluster"]));
        }

        static string Decode(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
    }
}
