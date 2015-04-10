using System;

namespace RElmah.Middleware.Bootstrapping.Builder
{
    public class ErrorsOptions
    {
        public Func<string> PrefixSetter { get; set; }
        public Func<bool> UseRandomizerSetter { get; set; }
    }
}