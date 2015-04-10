using System;
using RElmah.Domain;

namespace RElmah.Middleware.Bootstrapping.Builder
{
    public class ConfigurationOptions
    {
        public Func<string> PrefixSetter { get; set; }
        
    }
}