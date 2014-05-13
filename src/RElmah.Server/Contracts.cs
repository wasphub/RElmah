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
        IEnumerable<Cluster> Clusters { get; }
        IEnumerable<string> ExtractGroups(ErrorPayload payload);
    }

    public interface IDependencyRegistry
    {
        void Register(Type serviceType, Type implementationType);
        void RegisterAsSingleton(Type serviceType, Type implementationType);
    }
}
