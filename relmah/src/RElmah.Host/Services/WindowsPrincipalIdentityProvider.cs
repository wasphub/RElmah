using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.SignalR;

namespace RElmah.Host.Services
{
    public class WindowsPrincipalIdentityProvider : IIdentityProvider
    {
        public IIdentity GetIdentity(object request)
        {
            var r = request as IRequest;
            return r != null ? r.User.Identity : new ClaimsIdentity();
        }
    }
}