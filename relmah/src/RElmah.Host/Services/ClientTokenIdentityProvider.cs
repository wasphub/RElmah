using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.SignalR;

namespace RElmah.Host.Services
{
    public class ClientTokenIdentityProvider : IIdentityProvider
    {
        public IIdentity GetIdentity(object request)
        {
            var r = request as IRequest;

            return new ClaimsIdentity(
                r != null
                ? new []
                  {
                      new Claim(ClaimTypes.Name, r.QueryString["user"])
                  }
                : Enumerable.Empty<Claim>(), 
                "RElmah Client Token");
        }
    }
}