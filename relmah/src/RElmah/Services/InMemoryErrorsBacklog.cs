using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Common.Model;
using RElmah.Errors;
using RElmah.Foundation;

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

        public Task<ValueOrError<Recap>> GetSourcesRecap(IEnumerable<Source> sources, Func<IEnumerable<ErrorPayload>, int> reducer)
        {
            var errors =
                from e in _errors
                where sources.Select(a => a.SourceId).Contains(e.SourceId)
                select e;

            var grouped =
                from e in errors
                group e by e.SourceId into g
                let types = 
                    from t in g
                    group t by t.Error.Type into x
                    select new Recap.Type(x.Key, reducer(x))
                select new Recap.Source(g.Key, types);

            return Task.FromResult(new ValueOrError<Recap>(new Recap(DateTime.UtcNow, grouped)));
        }
    }
}
