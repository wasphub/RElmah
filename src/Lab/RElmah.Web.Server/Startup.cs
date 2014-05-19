using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using RElmah.Server.Extensions;

[assembly: OwinStartup(typeof(RElmah.Web.Server.Startup))]

namespace RElmah.Web.Server
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

            app.UseRElmah();
        }
    }
}
