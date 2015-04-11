using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using RElmah.Errors;
using RElmah.Visibility;

namespace RElmah.Middleware
{
    public class RandomSourceErrorsMiddleware : ErrorsMiddleware
    {
        private readonly IVisibilityPersistor _visibilityPersistor;
        private readonly Random _random = new Random();

        public RandomSourceErrorsMiddleware(OwinMiddleware next, IErrorsInbox inbox, IVisibilityPersistor visibilityPersistor, string prefix)
            : base(next, inbox, prefix)
        {
            _visibilityPersistor = visibilityPersistor;
        }

        protected override string RetrieveSourceId(IDictionary<string, string> form)
        {
            var sources = _visibilityPersistor.GetSources().Result.ToArray();
            var source  = sources.Skip(_random.Next(sources.Count())).First();
            return source.SourceId;
        }
    }
}