using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(RElmah.Server.Startup))]

namespace RElmah.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", builder =>
            {
                builder.UseCors(CorsOptions.AllowAll);
                builder.RunSignalR();
            });

            app.UseRElmah(cp => cp
                .AddCluster("c1")
                //.AddUserToCluster("wasp", "c1")
                .AddApplication("foo", "foo", "c1"));
        }
    }
}