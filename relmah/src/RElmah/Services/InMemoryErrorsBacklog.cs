using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Services
{
    public class InMemoryErrorsBacklog : IErrorsBacklog
    {
        private readonly AtomicImmutableList<ErrorPayload> _errors =
            new AtomicImmutableList<ErrorPayload>();

        public Task Store(ErrorPayload payload)
        {
            return Task.Factory.StartNew(() => _errors.Add(payload));
        }

        public Task<ValueOrError<Recap>> GetApplicationsRecap(IEnumerable<Application> apps, Func<IEnumerable<ErrorPayload>, int> processor)
        {
            var errors =
                from e in _errors
                where apps.Select(a => a.Name).Contains(e.SourceId)
                select e;

            var grouped =
                from e in errors
                group e by e.SourceId into g
                let types = 
                    from t in g
                    group t by t.Error.Type into x
                    select new Recap.Type(x.Key, processor(x))
                select new Recap.Application(g.Key, types);

            return Task.FromResult(new ValueOrError<Recap>(new Recap(DateTime.UtcNow, grouped)));
        }
    }
}
