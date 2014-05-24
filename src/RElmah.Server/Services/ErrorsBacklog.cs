using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Server.Domain;

namespace RElmah.Server.Services
{
    public class ErrorsBacklog : IErrorsBacklog
    {
        readonly ConcurrentBag<ErrorPayload> _errorPayloads = new ConcurrentBag<ErrorPayload>(); 

        public Task Store(ErrorPayload payload)
        {
            return Task.Factory.StartNew(() => _errorPayloads.Add(payload));
        }

        public Task<IEnumerable<ErrorPayload>> GetErrors()
        {
            return Task.FromResult((IEnumerable<ErrorPayload>)_errorPayloads);
        }
    }
}
