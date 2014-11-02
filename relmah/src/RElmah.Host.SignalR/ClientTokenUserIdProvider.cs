using Microsoft.AspNet.SignalR;

namespace RElmah.Host.SignalR
{
    class ClientTokenUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return request.QueryString["user"];
        }
    }
}
