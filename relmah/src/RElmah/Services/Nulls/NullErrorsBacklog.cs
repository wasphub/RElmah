using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Common.Model;
using RElmah.Errors;
using RElmah.Foundation;

namespace RElmah.Services.Nulls
{
    class NullErrorsBacklog : IErrorsBacklogWriter
    {
        private NullErrorsBacklog() { }

        public static IErrorsBacklogWriter Instance = new NullErrorsBacklog();

        public Task Store(ErrorPayload payload)
        {
            return Task.FromResult((object)null);
        }

        public Task<ValueOrError<Recap>> GetSourcesRecap(IEnumerable<Source> sources, Func<IEnumerable<ErrorPayload>, int> reducer)
        {
            return Task.FromResult((ValueOrError<Recap>)null);
        }
    }
}
