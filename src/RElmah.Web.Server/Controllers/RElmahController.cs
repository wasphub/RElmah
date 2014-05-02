using System.Net;
using System.Net.Http;
using System.Web.Http;
using RElmah.Server;
using RElmah.Server.Domain;

namespace RElmah.Web.Server.Controllers
{
    [Route("/post-error")]
    public class RElmahController : ApiController
    {
        private readonly IErrorsInbox _inbox;

        public RElmahController(IErrorsInbox inbox)
        {
            _inbox = inbox;
        }

        [HttpPost]
        public HttpResponseMessage Post(string error)
        {
            _inbox.Post(new ErrorPayload { Message = error });
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}
