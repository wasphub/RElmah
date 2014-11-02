using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
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

        public async static Task<ErrorPayload> Elmah(IDictionary<string, object> environment)
        {
            var @params = await new OwinContext(environment).Request.ReadFormAsync();

            var errorText = Decode(@params["error"]);
            var sourceId = @params["sourceId"];
            var errorId = @params["errorId"];
            var infoUrl = @params["infoUrl"];

            return new ErrorPayload(sourceId, JsonConvert.DeserializeObject<Error>(errorText), errorId, infoUrl);
        }

        static string Decode(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
    }
}
