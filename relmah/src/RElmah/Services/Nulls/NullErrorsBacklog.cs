using System.Threading.Tasks;
using RElmah.Common;

namespace RElmah.Services.Nulls
{
    class NullErrorsBacklog : IErrorsBacklog
    {
        public Task Store(ErrorPayload payload)
        {
            return Task.FromResult((object)null);
        }
    }
}
