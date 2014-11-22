using Microsoft.AspNet.SignalR;

namespace RElmah.Host.Services
{
    class ClientTokenUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return request.User.Identity.IsAuthenticated 
                 ? request.User.Identity.Name 
                 : request.QueryString["user"];
        }
    }
}
