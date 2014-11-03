using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;

namespace RElmah.Services.Nulls
{
    class NullConfigurationUpdater : IConfigurationUpdater
    {
        public Task<ValueOrError<Cluster>> AddCluster(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<bool>> RemoveCluster(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Cluster>> GetClusters()
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<Cluster>> GetCluster(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<Application>> AddApplication(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<bool>> RemoveApplication(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Application>> GetApplications()
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<Application>> GetApplication(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<User>> AddUser(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<bool>> RemoveUser(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<User>> GetUsers()
        {
            throw new System.NotImplementedException();
        }

        public Task<ValueOrError<User>> GetUser(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
