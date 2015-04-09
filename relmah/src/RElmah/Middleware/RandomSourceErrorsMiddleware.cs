using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using RElmah.Domain;
using RElmah.Errors;
using RElmah.Models.Settings;

namespace RElmah.Middleware
{
    public class RandomSourceErrorsMiddleware : ErrorsMiddleware
    {
        private readonly IDomainPersistor _domainPersistor;
        private readonly Random _random = new Random();

        public RandomSourceErrorsMiddleware(OwinMiddleware next, IErrorsInbox inbox, IDomainPersistor domainPersistor, ErrorsSettings settings)
            : base(next, inbox, settings)
        {
            _domainPersistor = domainPersistor;
        }

        protected override string RetrieveSourceId(IDictionary<string, string> form)
        {
            var sources = _domainPersistor.GetSources().Result.ToArray();
            var source  = sources.Skip(_random.Next(sources.Count())).First();
            return source.SourceId;
        }
    }
}