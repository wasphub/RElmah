using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using RElmah.Common;
using RElmah.Extensions;

namespace RElmah.Middleware
{
    public class RElmahMiddleware : OwinMiddleware
    {
        public override Task Invoke(IOwinContext context)
        {
            return Router.Invoke(context, Next.Invoke);
        }

        public RElmahMiddleware(OwinMiddleware next, IResolver resolver)
            : base(next)
        {
            var inbox   = new Lazy<IErrorsInbox>(resolver.Resolve<IErrorsInbox>);
            var updater = new Lazy<IConfigurationUpdater>(resolver.Resolve<IConfigurationUpdater>);

            Router.Build(builder => builder

                .ForRoute("clusters/{cluster}/apps/{app}", route => route
                    .Get(async (environment, keys) =>
                    {
                        var cluster = await updater.Value.GetCluster(keys["cluster"]);
                        return cluster.HasValue 
                             ? cluster.Value.GetApplication(keys["app"])
                             : null;
                    })
                    .Delete(async (environment, keys) => 
                        await updater.Value.RemoveApplicationFromCluster(keys["cluster"], keys["app"]))
                )
                .ForRoute("clusters/{cluster}/apps", route => route
                    .Post(async (environment, keys, form) => 
                        await updater.Value.AddApplicationToCluster(keys["cluster"], form["name"]))
                    .Get(async (environment, keys) =>
                    {
                        var cluster = await updater.Value.GetCluster(keys["cluster"]);
                        return cluster.HasValue
                             ? cluster.Value.Applications
                             : null;
                    })
                )

                //AD users
                .ForRoute("clusters/{cluster}/users/{domain}/{user}", route => route
                    .Get(async (environment, keys) =>
                    {
                        var cluster = await updater.Value.GetCluster(keys["cluster"]);
                        return cluster.HasValue
                             ? cluster.Value.GetUser(string.Format(@"{0}\{1}", keys["domain"], keys["user"]))
                             : null;
                    })
                    .Delete(async (environment, keys) =>
                        await updater.Value.RemoveUserFromCluster(keys["cluster"], string.Format(@"{0}\{1}", keys["domain"], keys["user"])))
                )
                //plain users
                .ForRoute("clusters/{cluster}/users/{user}", route => route
                    .Get(async (environment, keys) =>
                    {
                        var cluster = await updater.Value.GetCluster(keys["cluster"]);
                        return cluster.HasValue
                             ? cluster.Value.GetUser(keys["user"])
                             : null;
                    })
                    .Delete(async (environment, keys) =>
                        await updater.Value.RemoveUserFromCluster(keys["cluster"], keys["user"]))
                )
                .ForRoute("clusters/{cluster}/users", route => route
                    .Post(async (environment, keys, form) => 
                        await updater.Value.AddUserToCluster(keys["cluster"], form["name"]))
                    .Get(async (environment, keys) =>
                    {
                        var cluster = await updater.Value.GetCluster(keys["cluster"]);
                        return cluster.HasValue
                             ? cluster.Value.Users
                             : null;
                    })
                )

                .ForRoute("clusters/{cluster}", route => route
                    .Get(async (environment, keys) => 
                        await updater.Value.GetCluster(keys["cluster"]))
                    .Delete(async (environment, keys) =>
                        await updater.Value.RemoveCluster(keys["cluster"]))
                )
                .ForRoute("clusters", route => route
                    .Post(async (environment, keys, form) => 
                        await updater.Value.AddCluster(form["name"]))
                    .Get(async (environment, keys) => 
                        await updater.Value.GetClusters())
                )

                .ForRoute("users/{user}", route => route
                    .Get(async (environment, keys) =>
                        await updater.Value.GetUser(keys["user"]))
                    .Delete(async (environment, keys) =>
                        await updater.Value.RemoveUser(keys["user"]))
                )
                .ForRoute("users", route => route
                    .Post(async (environment, keys, form) => 
                        await updater.Value.AddUser(form["name"]))
                    .Get(async (environment, keys) =>
                        await updater.Value.GetUsers())
                )

                .ForRoute("apps/{app}", route => route
                    .Get(async (environment, keys) =>
                        await updater.Value.GetApplication(keys["app"]))
                    .Delete(async (environment, keys) =>
                        await updater.Value.RemoveApplication(keys["app"]))
                )
                .ForRoute("apps", route => route
                    .Post(async (environment, keys, form) => 
                        await updater.Value.AddApplication(form["name"]))
                    .Get(async (environment, keys) =>
                        await updater.Value.GetApplications())
                )

                .ForRoute("post-error", route => route
                    .Post(async (environment, keys, form) =>
                    {
                        var errorText = Encoding.UTF8.GetString(Convert.FromBase64String(form.Get("error")));
                        var sourceId  = form.Get("sourceId");
                        var errorId   = form.Get("errorId");
                        var infoUrl   = form.Get("infoUrl");

                        var payload   = new ErrorPayload(sourceId, JsonConvert.DeserializeObject<Error>(errorText), errorId, infoUrl);

                        await inbox.Value.Post(payload);

                        return payload;
                    })
                )
            );
        }
    }
}