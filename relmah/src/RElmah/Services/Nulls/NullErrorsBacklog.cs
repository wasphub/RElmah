﻿using System.Threading.Tasks;
using RElmah.Models.Errors;

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