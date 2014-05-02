using System.Threading.Tasks;
using RElmah.Server.Domain;

namespace RElmah.Server.Services
{
    public class NullErrorsBacklog : IErrorsBacklog
    {
        public Task Store(ErrorPayload payload)
        {
            return Task.FromResult<object>(null);
        }
    }
}
