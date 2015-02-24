using System.Collections.Generic;
using RElmah.Common;
using RElmah.Models;
using RElmah.Notifiers;

namespace RElmah.Host.Hubs
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

        public void UserApplications(string user, IEnumerable<string> added, IEnumerable<string> removed)
        {
            ErrorsHub.UserApplications(user, added, removed);
        }

        public void AddGroup(string token, string @group)
        {
            ErrorsHub.AddGroup(token, @group);
        }

        public void RemoveGroup(string token, string @group)
        {
            ErrorsHub.RemoveGroup(token, @group);
        }
    }
}