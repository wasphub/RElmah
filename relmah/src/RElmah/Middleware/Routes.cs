using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using RElmah.Common;

namespace RElmah.Middleware
{
    public static class Routes
    {
        public async static Task PostError(
            IErrorsInbox inbox,
            IDictionary<string, object> environment,
            IDictionary<string, string> keys = null)
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
            IDictionary<string, object> environment,
            IDictionary<string, string> keys = null)
        {
            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                var form = await request.ReadFormAsync();
                await updater.AddCluster(form["name"]);
                return;
            }

            if (keys == null) return;

            var r = keys.Keys.Count == 1
                  ? (dynamic)(await updater.GetCluster(keys["cluster"]))
                  : await updater.GetClusters();

            await new OwinResponse(environment).WriteAsync(JsonConvert.SerializeObject(r));
        }

        public async static Task Applications(
            IConfigurationUpdater updater,
            IDictionary<string, object> environment,
            IDictionary<string, string> keys = null)
        {
            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                var form = await request.ReadFormAsync();
                await updater.AddApplication(form["name"]);
                return;
            }

            if (keys == null) return;

            var r = keys.Keys.Count == 1
                  ? (dynamic)(await updater.GetApplication(keys["app"]))
                  : await updater.GetApplications();

            await new OwinResponse(environment).WriteAsync(JsonConvert.SerializeObject(r));
        }

        public async static Task Users(
            IConfigurationUpdater updater,
            IDictionary<string, object> environment,
            IDictionary<string, string> keys = null)
        {
            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                var form = await request.ReadFormAsync();
                await updater.AddUser(form["name"]);
                return;
            }

            if (keys == null) return;

            var r = keys.Keys.Count == 1
                  ? (dynamic)(await updater.GetUser(keys["user"]))
                  : await updater.GetUsers();

            await new OwinResponse(environment).WriteAsync(JsonConvert.SerializeObject(r));
        }

        public async static Task ClusterUsers(
            IConfigurationUpdater updater,
            IDictionary<string, object> environment,
            IDictionary<string, string> keys = null)
        {
            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                var form = await request.ReadFormAsync();
                await updater.AddUserToCluster(form["cluster"], form["user"]);
                return;
            }

            if (keys == null || keys.Keys.Count < 1) return;

            if (request.Method == "DELETE")
            {
                await updater.RemoveUserFromCluster(keys["cluster"], keys["user"]);
                return;
            }

            var cluster = await updater.GetCluster(keys["cluster"]);

            if (!cluster.HasValue) return;

            var r = keys.Keys.Count == 2
                  ? (dynamic)cluster.Value.GetUser(keys["user"])
                  : cluster.Value.Users;

            await new OwinResponse(environment).WriteAsync(JsonConvert.SerializeObject(r));
        }

        public async static Task ClusterApplications(
            IConfigurationUpdater updater,
            IDictionary<string, object> environment,
            IDictionary<string, string> keys = null)
        {
            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                var form = await request.ReadFormAsync();
                await updater.AddApplicationToCluster(form["cluster"], form["app"]);
                return;
            }

            if (keys == null || keys.Keys.Count < 1) return;

            if (request.Method == "DELETE" && keys.Keys.Count == 2)
            {
                await updater.RemoveApplicationFromCluster(keys["cluster"], keys["app"]);
                return;
            }

            var cluster = await updater.GetCluster(keys["cluster"]);

            if (!cluster.HasValue) return;

            var r = keys.Keys.Count == 2
                  ? (dynamic)cluster.Value.GetApplication(keys["app"])
                  : cluster.Value.Applications;

            await new OwinResponse(environment).WriteAsync(JsonConvert.SerializeObject(r));
        }
    }
}
