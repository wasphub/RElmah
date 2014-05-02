using System.Net;
using System.Net.Http;
using System.Web.Http;
using RElmah.Server;
using RElmah.Server.Domain;

namespace RElmah.Web.Server.Controllers
{
    public class RElmahController : ApiController
    {
        private readonly IErrorsInbox _inbox;

        public RElmahController(IErrorsInbox inbox)
        {
            _inbox = inbox;
        }

        [HttpPost]
        public HttpResponseMessage PostError(HttpRequestMessage request)
        {
            _inbox.Post(new ErrorPayload { Message = "Foo" });

            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}
