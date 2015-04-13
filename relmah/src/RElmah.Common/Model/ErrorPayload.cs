namespace RElmah.Common.Model
{
    public class ErrorPayload
    {
        public ErrorPayload(string sourceId, Error error, string errorId, string infoUrl)
        {
            SourceId = sourceId;
            Error = error;
            ErrorId = errorId;
            InfoUrl = infoUrl;
        }

        public string SourceId { get; private set; }
        public string ErrorId { get; private set; }
        public string InfoUrl { get; private set; }

        public Error Error { get; private set; }
    }
}
