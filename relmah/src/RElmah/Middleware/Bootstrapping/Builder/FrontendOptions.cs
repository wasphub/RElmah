using System;

namespace RElmah.Middleware.Bootstrapping.Builder
{
    public class FrontendOptions
    {
        public Func<Uri> TargetBackendEndpointSetter { get; set; }
    }
}