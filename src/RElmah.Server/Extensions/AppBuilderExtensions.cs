using Owin;

namespace RElmah.Server.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder MapRElmah(this IAppBuilder appBuilder)
        {
            appBuilder.MapSignalR();

            return appBuilder;
        }

        public static IAppBuilder RunRElmah(this IAppBuilder appBuilder)
        {
            appBuilder.RunSignalR();

            return appBuilder;
        }
    }
}
