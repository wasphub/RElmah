using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using RElmah.Models.Settings;

namespace RElmah.Middleware
{
    public class RandomSourceErrorsMiddleware : ErrorsMiddleware
    {
        private readonly IDomainPersistor _domainPersistor;
        private readonly Random _random = new Random();

        public RandomSourceErrorsMiddleware(OwinMiddleware next, IResolver resolver, ErrorsSettings settings) : base(next, resolver, settings)
        {
            _domainPersistor = resolver.Resolve<IDomainPersistor>();
        }

        protected override string RetrieveSourceId(IDictionary<string, string> form)
        {
            var apps = _domainPersistor.GetApplications().Result.ToArray();
            var app  = apps.Skip(_random.Next(apps.Count())).First();
            return app.Name;
        }
    }
}