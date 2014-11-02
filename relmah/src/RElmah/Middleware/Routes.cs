using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using RElmah.Models.Errors;

namespace RElmah.Middleware
{
    public static class Routes
    {
        public async static Task PostError(
            IErrorsInbox inbox,
            IDictionary<string, object> environment)
        {
            var executor = new Func<IDictionary<string, object>, Task<ErrorPayload>>(async e =>
            {
                var @params = await new OwinContext(e).Request.ReadFormAsync();

                var errorText = Encoding.UTF8.GetString(Convert.FromBase64String(@params["error"]));
                var sourceId  = @params["sourceId"];
                var errorId   = @params["errorId"];
                var infoUrl   = @params["infoUrl"];

                return new ErrorPayload(sourceId, JsonConvert.DeserializeObject<Error>(errorText), errorId, infoUrl);
            });

            await inbox.Post(await executor(environment));
        }

        public async static Task Clusters(
            IConfigurationUpdater updater,
            IConfigurationProvider provider,
            IDictionary<string, object> environment)
        {
            var build = new Func<OwinRequest, Task<string>>(async r =>
            {
                var @params = await r.ReadFormAsync();
                return await Task.FromResult(@params["name"]);
            });

            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                await updater.AddCluster(await build(request));
                return;
            }

            var response = new OwinResponse(environment);
            await response.WriteAsync(JsonConvert.SerializeObject(
                request.Uri.Segments.Count() > 3
                ? (dynamic)updater.GetCluster(request.Uri.Segments.Skip(3).First())
                : updater.GetClusters()));
        }
    }
}
