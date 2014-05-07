using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using RElmah.Domain;

namespace RElmah.Server.Middleware.Handlers
{
    public class ElmahDispatcher : PostDispatcher
    {
        public ElmahDispatcher(IErrorsInbox inbox) : base(inbox) { }

        static string Decode(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        protected override async Task<ErrorPayload> OnProcessRequest(IDictionary<string, object> environment)
        {
            var @params = await new OwinContext(environment).Request.ReadFormAsync();

            var errorText = Decode(@params["error"]);
            var sourceId  = @params["sourceId"];
            var errorId   = @params["errorId"];
            var infoUrl   = @params["infoUrl"];

            var e = JsonConvert.DeserializeObject<ErrorDetail>(errorText);

            return new ErrorPayload
            {
                ApplicationName = sourceId,
                Error = e
            };
        }
    }
}