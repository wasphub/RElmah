using System;

namespace RElmah.Host
{
    public class Connector : IConnector
    {
        private readonly IDomainWriter  _domainWriter;

        public Connector(IDomainWriter domainWriter)
        {
            _domainWriter  = domainWriter;
        }

        public void Connect(string user, string token, Action<string> connector)
        {
            _domainWriter.AddUserToken(user, token);

            foreach (var app in _domainWriter.GetUserApplications(user))
                connector(app.Name);
        }
    }
}