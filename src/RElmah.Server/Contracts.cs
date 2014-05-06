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
        IList<string> GetGroups(ErrorPayload payload);
    }
}
