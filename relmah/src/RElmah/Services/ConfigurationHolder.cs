﻿using System;
using System.Threading.Tasks;
using RElmah.Models;
using RElmah.Models.Configuration;

namespace RElmah.Services
{
    public class ConfigurationHolder : IConfigurationProvider, IConfigurationUpdater
    {
        public IObservable<Delta<Cluster>> ObserveClusters()
        {
            throw new NotImplementedException();
        }

        public Task<ValueOrError<Cluster>> AddCluster(string name)
        {
            throw new NotImplementedException();
        }

        public Task<ValueOrError<bool>> RemoveCluster(string name)
        {
            throw new NotImplementedException();
        }
    }
}