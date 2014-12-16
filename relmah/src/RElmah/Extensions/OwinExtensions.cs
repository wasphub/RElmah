using Microsoft.Owin;

namespace RElmah.Extensions
{
    public static class OwinExtensions
    {
        public static bool HasForm(this OwinRequest request)
        {
            return request.Method == "POST";
        }
    }
}
