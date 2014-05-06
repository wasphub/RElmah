using System.Threading.Tasks;
using RElmah.Domain;

namespace RElmah.Server.Services.Nulls
{
    public class NullErrorsDispatcher : IErrorsDispatcher
    {
        public Task Dispatch(ErrorPayload payload)
        {
            return Task.FromResult<object>(null);
        }
    }
}
