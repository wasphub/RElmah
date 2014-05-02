using System;
using RElmah.Server.Domain;

namespace RElmah.Server
{
    public interface IErrorsInbox
    {
        void Post(ErrorDescriptor descriptor);
        IObservable<ErrorDescriptor> GetErrors();
    }
}
