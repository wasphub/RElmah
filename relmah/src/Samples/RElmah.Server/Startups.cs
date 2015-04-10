﻿using System;
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
                    .RunFrontend()
                    .ForErrors()
                    .ForDomain(new DomainOptions
                    {
                        DomainConfigurator = async conf =>
                        {
                            var cFoo = await conf.AddCluster("foo");

                            var e7001 = await conf.AddSource("e7001");

                            //For Windows Auth testing
                            var cu = await conf.AddUser(string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName));

                            //For client token testing
                            var wasp = await conf.AddUser("wasp");

                            Task.WaitAll(
                                conf.AddSourceToCluster(cFoo.Value.Name, e7001.Value.SourceId),

                                conf.AddUserToCluster(cFoo.Value.Name, cu.Value.Name),
                                conf.AddUserToCluster(cFoo.Value.Name, wasp.Value.Name)
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
                    .RunFrontend()
                    .ForErrors(new ErrorsOptions { UseRandomizerSetter = () => true })
                    .ForDomain(new DomainOptions
                    {
                        DomainConfigurator = async conf =>
                        {
                            var cFoo = await conf.AddCluster("foo");
                            var cBar = await conf.AddCluster("bar");

                            var e7001 = await conf.AddSource("e7001");
                            var e7002 = await conf.AddSource("e7002");
                            var e7003 = await conf.AddSource("e7003");

                            //For Windows Auth testing
                            var cu = await conf.AddUser(string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName));

                            //For client token testing
                            var wasp = await conf.AddUser("wasp");
                            var cuki = await conf.AddUser("cuki");
                            var all = await conf.AddUser("all");

                            Task.WaitAll(
                                conf.AddSourceToCluster(cFoo.Value.Name, e7001.Value.SourceId),
                                conf.AddSourceToCluster(cBar.Value.Name, e7002.Value.SourceId),
                                conf.AddSourceToCluster(cBar.Value.Name, e7003.Value.SourceId),

                                conf.AddUserToCluster(cFoo.Value.Name, cu.Value.Name),
                                conf.AddUserToCluster(cFoo.Value.Name, wasp.Value.Name),
                                conf.AddUserToCluster(cFoo.Value.Name, all.Value.Name),

                                conf.AddUserToCluster(cBar.Value.Name, cuki.Value.Name),
                                conf.AddUserToCluster(cBar.Value.Name, all.Value.Name)
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