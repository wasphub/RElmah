using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Common;
using RElmah.Errors;
using RElmah.Foundation;
using RElmah.Models;

namespace RElmah.Services.Nulls
{
    class NullErrorsBacklog : IErrorsBacklog
    {
        private NullErrorsBacklog() { }

        public static IErrorsBacklog Instance = new NullErrorsBacklog();

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
