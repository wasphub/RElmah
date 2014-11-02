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
                .UseRElmah();
        }
    }
}