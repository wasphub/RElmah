using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Domain;

namespace RElmah.Server
{
    public interface IErrorsInbox
    {
        void Post(ErrorPayload payload);
        IObservable<ErrorPayload> GetErrors();
    }

    public interface IErrorsBacklog
    {
        Task Store(ErrorPayload payload);
    }

    public interface IErrorsDispatcher
    {
        Task Dispatch(ErrorPayload payload);
    }

    public interface IConfigurationProvider
    {
        IEnumerable<string> ExtractGroups(ErrorPayload payload);
        IEnumerable<Cluster> Clusters { get; }
        void AddCluster(Cluster cluster);
        Cluster GetCluster(string name);
    }

    public interface IConfigurationDispatcher
    {
        Task Dispatch(Cluster cluster);
    }

    public interface IDependencyRegistry
    {
        IDependencyRegistry Register(Type serviceType, Type implementationType);
        IDependencyRegistry RegisterAsSingleton(Type serviceType, Type implementationType);
    }
}
