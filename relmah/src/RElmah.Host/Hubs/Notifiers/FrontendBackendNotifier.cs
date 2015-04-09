using System.Net;
using Microsoft.AspNet.SignalR.Client;
using RElmah.Common;
using RElmah.Domain;
using RElmah.Errors;
using RElmah.Foundation;
using RElmah.Models;
using RElmah.Notifiers;

namespace RElmah.Host.Hubs.Notifiers
{
    public class FrontendBackendNotifier : IBackendNotifier
    {
        private readonly IHubProxy _proxy;

        public FrontendBackendNotifier(string endpoint, IErrorsInbox errorsInbox, IDomainPersistor domainPublisher)
        {
            var connection = new HubConnection(endpoint)
            {
                Credentials = CredentialCache.DefaultCredentials
            };

            _proxy = connection.CreateHubProxy("relmah-backend");

            _proxy.On<ErrorPayload>("error", p => errorsInbox.Post(p));

            _proxy.On<Delta<Cluster>>("cluster", p =>
            {
                switch (p.Type)
                {
                    case DeltaType.Added:   domainPublisher.AddCluster(p.Target.Name, true);    break;
                    case DeltaType.Removed: domainPublisher.RemoveCluster(p.Target.Name, true); break;
                }
            });

            _proxy.On<Delta<Source>>("source", p =>
            {
                switch (p.Type)
                {
                    case DeltaType.Added:   domainPublisher.AddSource(p.Target.SourceId, true); break;
                    case DeltaType.Removed: domainPublisher.RemoveSource(p.Target.SourceId, true); break;
                }
            });

            _proxy.On<Delta<User>>("user", p =>
            {
                switch (p.Type)
                {
                    case DeltaType.Added:   domainPublisher.AddUser(p.Target.Name, true); break;
                    case DeltaType.Removed: domainPublisher.RemoveUser(p.Target.Name, true); break;
                }
            });

            _proxy.On<Delta<Relationship<Cluster, Source>>>("clusterSource", p =>
            {
                switch (p.Type)
                {
                    case DeltaType.Added:   domainPublisher.AddSourceToCluster(p.Target.Primary.Name, p.Target.Secondary.SourceId, true); break;
                    case DeltaType.Removed: domainPublisher.RemoveSourceFromCluster(p.Target.Primary.Name, p.Target.Secondary.SourceId, true); break;
                }
            });

            _proxy.On<Delta<Relationship<Cluster, User>>>("clusterUser", p =>
            {
                switch (p.Type)
                {
                    case DeltaType.Added:   domainPublisher.AddUserToCluster(p.Target.Primary.Name, p.Target.Secondary.Name, true); break;
                    case DeltaType.Removed: domainPublisher.RemoveUserFromCluster(p.Target.Primary.Name, p.Target.Secondary.Name, true); break;
                }
            });

            connection.Start().Wait();
        }

        public void Error(ErrorPayload payload)
        {
            _proxy.Invoke("Error", payload);
        }

        public void Cluster(Delta<Cluster> payload)
        {
            _proxy.Invoke("Cluster", payload);
        }

        public void Source(Delta<Source> payload)
        {
            _proxy.Invoke("Source", payload);
        }

        public void User(Delta<User> payload)
        {
            _proxy.Invoke("User", payload);
        }

        public void ClusterSource(Delta<Relationship<Cluster, Source>> payload)
        {
            _proxy.Invoke("ClusterSource", payload);
        }

        public void ClusterUser(Delta<Relationship<Cluster, User>> payload)
        {
            _proxy.Invoke("ClusterUser", payload);
        }
    }
}