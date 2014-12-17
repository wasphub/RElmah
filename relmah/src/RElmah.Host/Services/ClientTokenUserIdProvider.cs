using System;
using Microsoft.AspNet.SignalR;

namespace RElmah.Host.Services
{
    public class ClientTokenUserIdProvider : IUserIdProvider
    {
        private readonly IIdentityProvider _identityProvider;

        public ClientTokenUserIdProvider(IIdentityProvider identityProvider)
        {
            _identityProvider = identityProvider;
        }

        public string GetUserId(IRequest request)
        {
            var identity = _identityProvider.GetIdentity(request);
            return identity.IsAuthenticated ? identity.Name : null;
        }
    }
}
