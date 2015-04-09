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
            var appSettings           = ConfigurationManager.AppSettings;

            var runBackend            = appSettings["runBackend"].IsTruthy();
            var winAuth               = appSettings["winAuth"].IsTruthy();
            var runErrors             = appSettings["runErrors"].IsTruthy(true);
            var runDomain             = appSettings["runDomain"].IsTruthy();

            var targetBackendEndpoint = appSettings["targetBackendEndpoint"];

            app
                .Map("/signalr", builder =>
                {
                    builder.UseCors(CorsOptions.AllowAll);
                    builder.RunSignalR();
                })

                .UseRElmah(new Settings
                {
                    Errors = runErrors.Then(() => new ErrorsSettings
                    {
                        Prefix = "relmah-errors"
                    }),

                    Domain = runDomain.Then(() => new DomainSettings
                    {
                        Prefix  = "relmah-domain"
                    }),

                    Bootstrap = new BootstrapperSettings
                    {       
                        //Enable the following line to use the basic client side token for authentication (for test purposes)
                        IdentityProviderBuilder = () => winAuth 
                                                        ? (IIdentityProvider)new WindowsPrincipalIdentityProvider() 
                                                        : new ClientTokenIdentityProvider(),

                        RunBackend = runBackend,
                        TargetBackendEndpoint = targetBackendEndpoint,

                        Domain = async conf =>
                        {              
                            var c1 = await conf.AddCluster("foo");
                            var c2 = await conf.AddCluster("bar");

                            var a1 = await conf.AddSource("e7001");
                            var a2 = await conf.AddSource("e7002");
                            var a3 = await conf.AddSource("e7003");

                            //For Windows Auth testing
                            var u1 = await conf.AddUser(string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName));
                            
                            //For client token testing
                            var u2 = await conf.AddUser(@"wasp");
                            var u3 = await conf.AddUser(@"cuki");
                            var u4 = await conf.AddUser(@"all");

                            Task.WaitAll(
                                conf.AddSourceToCluster(c1.Value.Name, a1.Value.SourceId),
                                conf.AddSourceToCluster(c2.Value.Name, a2.Value.SourceId),
                                conf.AddSourceToCluster(c2.Value.Name, a3.Value.SourceId),

                                conf.AddUserToCluster(c1.Value.Name, u1.Value.Name),
                                conf.AddUserToCluster(c1.Value.Name, u2.Value.Name),
                                conf.AddUserToCluster(c1.Value.Name, u4.Value.Name),

                                conf.AddUserToCluster(c2.Value.Name, u3.Value.Name),
                                conf.AddUserToCluster(c2.Value.Name, u4.Value.Name));
                        }
                    }
                });
        }
    }
}