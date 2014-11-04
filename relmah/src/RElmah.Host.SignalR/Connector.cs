using System;

namespace RElmah.Host.SignalR
{
    public class Connector : IConnector
    {
        private readonly IConfigurationUpdater  _configurationUpdater;

        public Connector(IConfigurationUpdater configurationUpdater)
        {
            _configurationUpdater  = configurationUpdater;
        }

        public void Connect(string user, Action<string> connector)
        {
            foreach (var app in _configurationUpdater.GetUserApplications(user))
                connector(app.Name);
        }
    }
}