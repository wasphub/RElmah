using System;
using System.Threading.Tasks;
using RElmah.Server.Domain;

namespace RElmah.Server
{
    public interface IErrorsInbox
    {
        void Post(ErrorDescriptor descriptor);
        IObservable<ErrorDescriptor> GetErrors();
    }

    public interface IErrorsBacklog
    {
        Task Store(ErrorDescriptor descriptor);
    }

    public interface IErrorsDispatcher
    {
        Task Dispatch(ErrorDescriptor descriptor);
    }
}
