using System.Collections.Generic;
using System.Net;
using Microsoft.AspNet.SignalR.Client;
using RElmah.Common;
using RElmah.Errors;
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

    public class BackendNotifier : IBackendNotifier
    {
        private readonly IHubProxy _proxy;

        public BackendNotifier(string endpoint, IErrorsInbox errorsInbox)
        {
            var connection = new HubConnection(endpoint)
            {
                Credentials = CredentialCache.DefaultCredentials
            };

            _proxy = connection.CreateHubProxy("relmah-backend");

            _proxy.On<ErrorPayload>("error", p => errorsInbox.Post(p));

            connection.Start().Wait();
        }

        public void Error(ErrorPayload payload)
        {
            _proxy.Invoke("Error", payload);
        }
    }
}