using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using RElmah.Host;

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
                    ExposeConfigurationWebApi = true,
                    BootstrapConfiguration = async cu =>
                    {
                        var c  = await cu.AddCluster("foo");
                        var a  = await cu.AddApplication("sample");
                        var u1 = await cu.AddUser(@"WaspBookWin81\wasp");
                        var u2 = await cu.AddUser(@"wasp");

                        Task.WaitAll(
                            cu.AddApplicationToCluster(c.Value.Name, a.Value.Name),
                            cu.AddUserToCluster(c.Value.Name, u1.Value.Name),
                            cu.AddUserToCluster(c.Value.Name, u2.Value.Name));
                    }
                });
        }
    }
}