using RElmah.Common;

namespace RElmah.Server.Domain
{
    public class ErrorPayload
    {
        public ErrorPayload(string sourceId, ErrorDetail detail, string errorId, string infoUrl)
        {
            SourceId = sourceId;
            Detail = detail;
            ErrorId = errorId;
            InfoUrl = infoUrl;
        }

        public string SourceId { get; private set; }
        public string ErrorId { get; private set; }
        public string InfoUrl { get; private set; }

        public ErrorDetail Detail { get; private set; }
    }
}
