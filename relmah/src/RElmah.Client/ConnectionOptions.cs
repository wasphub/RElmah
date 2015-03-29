using System;
using RElmah.Common;

namespace RElmah.Client
{
    public class ConnectionOptions
    {
        public Func<IObservable<ErrorPayload>, IObservable<ErrorPayload>> ErrorsFilter { get; set; }     
    }
}