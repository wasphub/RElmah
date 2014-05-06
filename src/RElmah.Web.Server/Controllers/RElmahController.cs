using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RElmah.Domain;
using RElmah.Server;

namespace RElmah.Web.Server.Controllers
{
    public class RElmahController : ApiController
    {
        private readonly IErrorsInbox _inbox;
        private readonly Exception[] _randoms =
        {
            new OutOfMemoryException("No more memory..."), 
            new ArgumentNullException("foo"), 
            new InvalidCastException("What more are you trying to achieve??"), 
            new StackOverflowException("Tail recursion rulez!"), 
            new AccessViolationException("Scary...!")
        };
        private readonly Random _randomizer = new Random();

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

        [HttpPost]
        public HttpResponseMessage RandomError(HttpRequestMessage request)
        {
            _inbox.Post(new ErrorPayload { Message = _randoms[_randomizer.Next(_randoms.Length)].Message });

            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}
