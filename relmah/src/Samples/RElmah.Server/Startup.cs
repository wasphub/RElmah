using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using RElmah.Host.SignalR;

[assembly: OwinStartup(typeof(RElmah.Server.Startup))]

namespace RElmah.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app
                .Map("/signalr", builder =>
                {
                    builder.UseCors(CorsOptions.AllowAll);
                    builder.RunSignalR();
                })
                .UseRElmah(new Settings
                {
                    InitializeConfiguration = async cu =>
                    {
                        var c = await cu.AddCluster("foo");
                        var a = await cu.AddApplication("argo");
                        var u = await cu.AddUser("wasp");

                        c.Value.AddApplication(a.Value);
                        c.Value.AddUser(u.Value);
                    }
                });
        }
    }
}