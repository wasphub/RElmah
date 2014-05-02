using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Server.Domain;

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
    }

    public interface IConfigurationProvider
    {
        IEnumerable<Cluster> Clusters { get; }
        IList<string> GetGroups(ErrorPayload payload);
    }
}
