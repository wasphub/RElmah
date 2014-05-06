using System.Threading.Tasks;
using RElmah.Domain;

namespace RElmah.Server.Services.Nulls
{
    public class NullErrorsBacklog : IErrorsBacklog
    {
        public Task Store(ErrorPayload payload)
        {
            return Task.FromResult<object>(null);
        }
    }
}
