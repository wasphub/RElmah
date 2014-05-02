using System.Threading.Tasks;
using RElmah.Server.Domain;

namespace RElmah.Server.Services
{
    public class ErrorsDispatcher : IErrorsDispatcher
    {
        private readonly IErrorsBacklog _errorsBacklog;

        public ErrorsDispatcher(IErrorsBacklog errorsBacklog)
        {
            _errorsBacklog = errorsBacklog;
        }

        public Task Dispatch(ErrorDescriptor descriptor)
        {
            return Task.FromResult<object>(null);
        }
    }
}
