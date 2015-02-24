using RElmah.Common;
using RElmah.Notifiers;

namespace RElmah.Services.Nulls
{
    public class NullBackendNotifier : IBackendNotifier
    {
        private NullBackendNotifier() { }

        public static IBackendNotifier Instance = new NullBackendNotifier();
 
        public void Error(ErrorPayload payload)
        {
        }
    }
}
