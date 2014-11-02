using System;
using System.Threading.Tasks;
using RElmah.Models.Errors;

namespace RElmah.Host.SignalR
{
    public class Dispatcher : IDispatcher
    {
        public Dispatcher(IErrorsInbox errorsInbox)
        {
            errorsInbox.GetErrorsStream().Subscribe(p => DispatchError(p));
        }

        public Task DispatchError(ErrorPayload payload)
        {
            throw new NotImplementedException();
        }
    }
}
