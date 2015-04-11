using System.Collections.Generic;
using RElmah.Common;
using RElmah.Models;
using RElmah.Notifiers;

namespace RElmah.Host.Hubs.Notifiers
{
    public class FrontendNotifier : IFrontendNotifier
    {
        public void Recap(string user, Recap recap)
        {
            ErrorsHub.Recap(user, recap);
        }

        public void Error(string user, ErrorPayload payload)
        {
            ErrorsHub.Error(user, payload);
        }

        public void UserSources(string user, IEnumerable<Source> added, IEnumerable<Source> removed)
        {
            ErrorsHub.UserSources(user, added, removed);
        }
    }
}