using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Owin.Cors;
using Owin;
using RElmah.Extensions;
using RElmah.Host.Extensions.AppBuilder;
using RElmah.Host.Services;
using RElmah.Middleware.Bootstrapping.Builder;

namespace RElmah.Server
{
    public class Startup_SimpleFrontendForErrorsAndDomain
    {
        public void Configuration(IAppBuilder app)
        {
            var appSettings           = ConfigurationManager.AppSettings;

            var winAuth               = appSettings["winAuth"].IsTruthy();

            app
                .Map("/signalr", builder =>
                {
                    builder.UseCors(CorsOptions.AllowAll);
                    builder.RunSignalR();
                })

                .UseRElmah(builder => builder
                    
                    .WithOptions(new BootstrapOptions
                    {
                        IdentityProviderBuilderSetter = () => 
                            () => winAuth
                                  ? (IIdentityProvider)new WindowsPrincipalIdentityProvider()
                                  : new ClientTokenIdentityProvider()
                    })
                    .RunFrontend()
                    .ReceiveErrors()
                    .ExposeConfiguration(new ConfigurationOptions
                    {
                        Configurator = async conf =>
                        {
                            var c01 = await conf.AddCluster("c01");

                            var s01 = await conf.AddSource("s01");

                            //For Windows Auth testing
                            var wcu = await conf.AddUser(string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName));

                            //For client token testing
                            var u01 = await conf.AddUser("u01");

                            Task.WaitAll(
                                conf.AddSourceToCluster(c01.Value.Name, s01.Value.SourceId),

                                conf.AddUserToCluster(c01.Value.Name, wcu.Value.Name),
                                conf.AddUserToCluster(c01.Value.Name, u01.Value.Name)
                            );
                        }
                    })
                    .Build()
                    
                );
        }
    }

    public class Startup_ComplexFrontendForErrorsAndDomain
    {
        public void Configuration(IAppBuilder app)
        {
            var appSettings = ConfigurationManager.AppSettings;

            var winAuth = appSettings["winAuth"].IsTruthy();

            var targetBackendEndpoint = appSettings["targetBackendEndpoint"];

            app
                .Map("/signalr", builder =>
                {
                    builder.UseCors(CorsOptions.AllowAll);
                    builder.RunSignalR();
                })

                .UseRElmah(builder => builder

                    .WithOptions(new BootstrapOptions
                    {
                        IdentityProviderBuilderSetter = () =>
                            () => winAuth
                                  ? (IIdentityProvider)new WindowsPrincipalIdentityProvider()
                                  : new ClientTokenIdentityProvider()
                    })
                    .RunFrontend(targetBackendEndpoint != null 
                                 ? new FrontendOptions
                                 {
                                     TargetBackendEndpointSetter = () => new Uri(targetBackendEndpoint)
                                 } 
                                 : null)
                    .ReceiveErrors(new ErrorsOptions { UseRandomizerSetter = () => true })
                    .ExposeConfiguration(new ConfigurationOptions
                    {
                        Configurator = async conf =>
                        {
                            var c01 = await conf.AddCluster("c01");
                            var c02 = await conf.AddCluster("c02");
                                    
                            var s01 = await conf.AddSource("s01");
                            var s02 = await conf.AddSource("s02");
                            var s03 = await conf.AddSource("s03");

                            //For Windows Auth testing
                            var wcu = await conf.AddUser(string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName));

                            //For client token testing
                            var u01 = await conf.AddUser("u01");
                            var u02 = await conf.AddUser("u02");
                            var all = await conf.AddUser("all");

                            Task.WaitAll(
                                conf.AddSourceToCluster(c01.Value.Name, s01.Value.SourceId),
                                conf.AddSourceToCluster(c02.Value.Name, s02.Value.SourceId),
                                conf.AddSourceToCluster(c02.Value.Name, s03.Value.SourceId),

                                conf.AddUserToCluster(c01.Value.Name, wcu.Value.Name),
                                conf.AddUserToCluster(c01.Value.Name, u01.Value.Name),
                                conf.AddUserToCluster(c01.Value.Name, all.Value.Name),

                                conf.AddUserToCluster(c02.Value.Name, u02.Value.Name),
                                conf.AddUserToCluster(c02.Value.Name, all.Value.Name)
                            );
                        }
                    })
                    .Build()

                );
        }
    }

    public class Startup_Backend
    {
        public void Configuration(IAppBuilder app)
        {
            app
                .Map("/signalr", builder =>
                {
                    builder.UseCors(CorsOptions.AllowAll);
                    builder.RunSignalR();
                })

                .UseRElmah(builder => builder

                    .WithOptions(new BootstrapOptions
                    {
                        IdentityProviderBuilderSetter = () =>
                            () => (IIdentityProvider)new WindowsPrincipalIdentityProvider()
                    })
                    .RunBackend()
                    .Build()

                );
        }
    }
}