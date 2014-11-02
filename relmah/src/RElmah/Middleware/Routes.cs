using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Models.Errors;

namespace RElmah.Middleware
{
    public static class Routes
    {
        public async static Task PostError(
            IErrorsInbox inbox,
            Func<IDictionary<string, object>, Task<ErrorPayload>> executor,
            IDictionary<string, object> environment)
        {
            await inbox.Post(await executor( environment));
        }
    }
}
