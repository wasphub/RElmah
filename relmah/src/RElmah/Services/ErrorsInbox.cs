using System;
using System.Threading.Tasks;
using RElmah.Models.Errors;

namespace RElmah.Services
{
    public class ErrorsInbox : IErrorsInbox
    {
        public Task Post(ErrorPayload payload)
        {
            throw new NotImplementedException();
        }

        public IObservable<ErrorPayload> GetErrorsStream()
        {
            throw new NotImplementedException();
        }
    }
}
