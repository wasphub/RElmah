using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;

namespace RElmah.Host.SignalR
{
    public class RElmahMiddleware : OwinMiddleware
    {
        public RElmahMiddleware(OwinMiddleware next, IDependencyResolver resolver)
            : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            return Next.Invoke(context);
        }
    }
}