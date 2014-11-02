using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
