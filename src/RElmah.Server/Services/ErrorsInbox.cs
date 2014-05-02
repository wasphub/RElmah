using System;
using RElmah.Server.Domain;

namespace RElmah.Server.Services
{
    class ErrorsInbox : IErrorsInbox
    {
        public ErrorsInbox()
        {
            
        }

        public void Post(ErrorDescriptor descriptor)
        {
            

        }

        public IObservable<ErrorDescriptor> GetErrors()
        {
            return null;
        }
    }
}
