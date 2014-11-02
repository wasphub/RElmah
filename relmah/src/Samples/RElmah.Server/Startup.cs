using Microsoft.Owin;
using Owin;
using RElmah.Host.SignalR;

[assembly: OwinStartup(typeof(RElmah.Server.Startup))]

namespace RElmah.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", builder =>
            {
                builder.RunSignalR();
            });

            app.UseRElmah();
        }
    }
}