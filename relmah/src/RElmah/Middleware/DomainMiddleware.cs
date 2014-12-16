using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace RElmah.Middleware
{
    public class DomainMiddleware : OwinMiddleware
    {
        public override Task Invoke(IOwinContext context)
        {
            return Router.Invoke(context, Next.Invoke);
        }

        public DomainMiddleware(OwinMiddleware next, IResolver resolver)
            : base(next)
        {
            var updater = new Lazy<IDomainWriter>(resolver.Resolve<IDomainWriter>);

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

                );
        }
    }
}