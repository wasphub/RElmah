using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using RElmah.Models.Settings;

namespace RElmah.Middleware
{
    public class DomainMiddleware : OwinMiddleware
    {
        public override Task Invoke(IOwinContext context)
        {
            return Router.Invoke(context, Next.Invoke);
        }

        public DomainMiddleware(OwinMiddleware next, IDomainPersistor updater, DomainSettings settings)
            : base(next)
        {
            Router.Build(builder => builder

                .WithPrefix(settings.Prefix)

                .ForRoute("clusters/{cluster}/apps/{app}", route => route
                    .Get(async (environment, keys, _) =>
                    {
                        var cluster = await updater.GetCluster(keys["cluster"]);
                        return cluster.HasValue 
                            ? cluster.Value.GetApplication(keys["app"])
                            : null;
                    })
                    .Delete(async (environment, keys, _) => 
                        await updater.RemoveApplicationFromCluster(keys["cluster"], keys["app"]))
                )
                .ForRoute("clusters/{cluster}/apps", route => route
                    .Post(async (environment, keys, form) => 
                        await updater.AddApplicationToCluster(keys["cluster"], form["name"]))
                    .Get(async (environment, keys, _) =>
                    {
                        var cluster = await updater.GetCluster(keys["cluster"]);
                        return cluster.HasValue
                            ? cluster.Value.Applications
                            : null;
                    })
                )

                //AD users
                .ForRoute("clusters/{cluster}/users/{domain}/{user}", route => route
                    .Get(async (environment, keys, _) =>
                    {
                        var cluster = await updater.GetCluster(keys["cluster"]);
                        return cluster.HasValue
                            ? cluster.Value.GetUser(string.Format(@"{0}\{1}", keys["domain"], keys["user"]))
                            : null;
                    })
                    .Delete(async (environment, keys, _) =>
                        await updater.RemoveUserFromCluster(keys["cluster"], string.Format(@"{0}\{1}", keys["domain"], keys["user"])))
                )
                //plain users
                .ForRoute("clusters/{cluster}/users/{user}", route => route
                    .Get(async (environment, keys, _) =>
                    {
                        var cluster = await updater.GetCluster(keys["cluster"]);
                        return cluster.HasValue
                            ? cluster.Value.GetUser(keys["user"])
                            : null;
                    })
                    .Delete(async (environment, keys, _) =>
                        await updater.RemoveUserFromCluster(keys["cluster"], keys["user"]))
                )
                .ForRoute("clusters/{cluster}/users", route => route
                    .Post(async (environment, keys, form) => 
                        await updater.AddUserToCluster(keys["cluster"], form["name"]))
                    .Get(async (environment, keys, _) =>
                    {
                        var cluster = await updater.GetCluster(keys["cluster"]);
                        return cluster.HasValue
                            ? cluster.Value.Users
                            : null;
                    })
                )

                .ForRoute("clusters/{cluster}", route => route
                    .Get(async (environment, keys, _) => 
                        await updater.GetCluster(keys["cluster"]))
                    .Delete(async (environment, keys, _) =>
                        await updater.RemoveCluster(keys["cluster"]))
                )
                .ForRoute("clusters", route => route
                    .Post(async (environment, keys, form) => 
                        await updater.AddCluster(form["name"]))
                    .Get(async (environment, keys, _) => 
                        await updater.GetClusters())
                )

                .ForRoute("users/{user}", route => route
                    .Get(async (environment, keys, _) =>
                        await updater.GetUser(keys["user"]))
                    .Delete(async (environment, keys, _) =>
                        await updater.RemoveUser(keys["user"]))
                )
                .ForRoute("users", route => route
                    .Post(async (environment, keys, form) => 
                        await updater.AddUser(form["name"]))
                    .Get(async (environment, keys, _) =>
                        await updater.GetUsers())
                )

                .ForRoute("apps/{app}", route => route
                    .Get(async (environment, keys, _) =>
                        await updater.GetApplication(keys["app"]))
                    .Delete(async (environment, keys, _) =>
                        await updater.RemoveApplication(keys["app"]))
                )
                .ForRoute("apps", route => route
                    .Post(async (environment, keys, form) => 
                        await updater.AddApplication(form["name"]))
                    .Get(async (environment, keys, _) =>
                        await updater.GetApplications())
                )

            );
        }
    }
}