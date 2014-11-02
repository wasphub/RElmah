using System;
using System.Threading.Tasks;
using RElmah.Models.Errors;

namespace RElmah
{
    /// <summary>
    /// This contract receives errors from the 
    /// outside world, 'parks' them and makes
    /// them observable. The 'parking' strategy
    /// is left to the implementor.
    /// </summary>
    public interface IErrorsInbox
    {
        Task Post(ErrorPayload payload);
        IObservable<ErrorPayload> GetErrorsStream();
    }
}
