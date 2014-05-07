using System;
using System.Collections.Generic;

namespace RElmah.Domain
{
    public class ErrorDetail
    {
        public string Id { get; set; }

        public string Host { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        public string User { get; set; }
        public string StatusCode { get; set; }
        public DateTime Time { get; set; }
        public string Url { get; set; }

        public string WebHostHtmlMessage { get; set; }
        public string Source { get; set; }
        public IDictionary<string, string> ServerVariables { get; set; }
        public IDictionary<string, string> Form { get; set; }
        public IDictionary<string, object> Cookies { get; set; }
    }

    public class ErrorPayload
    {
        public string ApplicationName { get; set; }
        public ErrorDetail Error { get; set; }
    }
}
