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

            var payload = request.Uri.Segments.Count() > 3
                ? (dynamic)(await updater.GetCluster(request.Uri.Segments.Skip(3).First()))
                : await updater.GetClusters();

            await new OwinResponse(environment).WriteAsync(JsonConvert.SerializeObject(payload));
        }

        public async static Task Applications(
            IConfigurationUpdater updater,
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
                await updater.AddApplication(await build(request));
                return;
            }

            var payload = request.Uri.Segments.Count() > 3
                ? (dynamic)(await updater.GetApplication(request.Uri.Segments.Skip(3).First()))
                : await updater.GetApplications();

            await new OwinResponse(environment).WriteAsync(JsonConvert.SerializeObject(payload));
        }

        public async static Task Users(
            IConfigurationUpdater updater,
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
                await updater.AddUser(await build(request));
                return;
            }

            var payload = request.Uri.Segments.Count() > 3
                ? (dynamic)(await updater.GetUser(request.Uri.Segments.Skip(3).First()))
                : await updater.GetUsers();

            await new OwinResponse(environment).WriteAsync(JsonConvert.SerializeObject(payload));
        }

        public async static Task ClusterUsers(
            IConfigurationUpdater updater,
            IDictionary<string, object> environment)
        {
            var build = new Func<OwinRequest, Task<Tuple<string, string>>>(async r =>
            {
                var @params = await r.ReadFormAsync();
                return await Task.FromResult(Tuple.Create(@params["cluster"], @params["user"]));
            });

            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                var form = await build(request);
                await updater.AddUserToCluster(form.Item1, form.Item2);
                return;
            }

            if (request.Method == "DELETE")
            {
                var form = await build(request);
                await updater.RemoveUserFromCluster(form.Item1, form.Item2);
                return;
            }

            var payload = request.Uri.Segments.Count() > 3
                ? (dynamic)(await updater.GetCluster(request.Uri.Segments.Skip(3).First()))
                : await updater.GetClusters();

            await new OwinResponse(environment).WriteAsync(JsonConvert.SerializeObject(payload));
        }

        public async static Task ClusterApplications(
            IConfigurationUpdater updater,
            IDictionary<string, object> environment)
        {
            var build = new Func<OwinRequest, Task<Tuple<string, string>>>(async r =>
            {
                var @params = await r.ReadFormAsync();
                return await Task.FromResult(Tuple.Create(@params["cluster"], @params["application"]));
            });

            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                var form = await build(request);
                await updater.AddApplicationToCluster(form.Item1, form.Item2);
                return;
            }

            if (request.Method == "DELETE")
            {
                var form = await build(request);
                await updater.RemoveApplicationFromCluster(form.Item1, form.Item2);
                return;
            }

            var payload = request.Uri.Segments.Count() > 3
                ? (dynamic)(await updater.GetCluster(request.Uri.Segments.Skip(3).First()))
                : await updater.GetClusters();

            await new OwinResponse(environment).WriteAsync(JsonConvert.SerializeObject(payload));
        }
    }
}
