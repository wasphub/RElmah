using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using RElmah.Common;
using RElmah.Extensions;
using RElmah.Models.Settings;

namespace RElmah.Middleware
{
    public class ErrorsMiddleware : OwinMiddleware
    {
        public override Task Invoke(IOwinContext context)
        {
            return Router.Invoke(context, Next.Invoke);
        }

        public ErrorsMiddleware(OwinMiddleware next, IErrorsInbox inbox, ErrorsSettings settings)
            : base(next)
        {
            Router.Build(builder => builder

                .WithPrefix(settings.Prefix)

                .ForRoute("post-error", route => route
                    .Post(async (environment, keys, form) =>
                    {
                        var errorText = Encoding.UTF8.GetString(Convert.FromBase64String(form.Get("error")));
                        var sourceId  = RetrieveSourceId(form);
                        var errorId   = form.Get("errorId");
                        var infoUrl   = form.Get("infoUrl");

                        var payload   = new ErrorPayload(sourceId, JsonConvert.DeserializeObject<Error>(errorText), errorId, infoUrl);

                        await inbox.Post(payload);

                        return payload;
                    })
                )
            );
        }

        protected virtual string RetrieveSourceId(IDictionary<string, string> form)
        {
            return form.Get("sourceId");
        }
    }
}