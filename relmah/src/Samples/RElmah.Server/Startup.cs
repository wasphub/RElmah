using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using RElmah.Extensions;
using RElmah.Host.Extensions.AppBuilder;
using RElmah.Host.Services;
using RElmah.Models.Settings;
using RElmah.Server;

[assembly: OwinStartup(typeof(Startup))]

namespace RElmah.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var appSettings = ConfigurationManager.AppSettings;

            var runBackend = appSettings["runBackend"].IsTruthy();
            var isTruthy = appSettings["winAuth"].IsTruthy();
            var targetBackendEndpoint = appSettings["targetBackendEndpoint"];

            app
                .Map("/signalr", builder =>
                {
                    builder.UseCors(CorsOptions.AllowAll);
                    builder.RunSignalR();
                })

                .UseRElmah(new Settings
                {
                    Errors = new ErrorsSettings
                    {
                        UseRandomizer = true,
                        Prefix = "relmah-errors"
                    },

                    Domain = new DomainSettings
                    {
                        Exposed = true,
                        Prefix  = "relmah-domain"
                    },

                    Bootstrap = new BootstrapperSettings
                    {       
                        //Enable the following line to use the basic client side token for authentication (for test purposes)
                        IdentityProviderBuilder = () => isTruthy 
                                                        ? (IIdentityProvider)new WindowsPrincipalIdentityProvider() 
                                                        : new ClientTokenIdentityProvider(),

                        RunBackend = runBackend,
                        TargetBackendEndpoint = targetBackendEndpoint,

                        Domain = async cu =>
                        {              
                            var c1 = await cu.AddCluster("foo");
                            var c2 = await cu.AddCluster("bar");

                            var a1 = await cu.AddApplication("e7001");
                            var a2 = await cu.AddApplication("e7002");
                            var a3 = await cu.AddApplication("e7003");

                            //For Windows Auth testing
                            var u1 = await cu.AddUser(string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName));
                            
                            //For client token testing
                            var u2 = await cu.AddUser(@"wasp");
                            var u3 = await cu.AddUser(@"cuki");
                            var u4 = await cu.AddUser(@"all");

                            Task.WaitAll(
                                cu.AddApplicationToCluster(c1.Value.Name, a1.Value.Name),
                                cu.AddApplicationToCluster(c2.Value.Name, a2.Value.Name),
                                cu.AddApplicationToCluster(c2.Value.Name, a3.Value.Name),

                                cu.AddUserToCluster(c1.Value.Name, u1.Value.Name),
                                cu.AddUserToCluster(c1.Value.Name, u2.Value.Name),
                                cu.AddUserToCluster(c1.Value.Name, u4.Value.Name),

                                cu.AddUserToCluster(c2.Value.Name, u3.Value.Name),
                                cu.AddUserToCluster(c2.Value.Name, u4.Value.Name));
                        }
                    }
                });
        }
    }
}