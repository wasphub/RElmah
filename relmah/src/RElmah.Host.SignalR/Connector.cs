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

        public void Connect(string user, string token, Action<string> connector)
        {
            _configurationUpdater.AddUserToken(user, token);

            foreach (var app in _configurationUpdater.GetUserApplications(user))
                connector(app.Name);
        }
    }
}