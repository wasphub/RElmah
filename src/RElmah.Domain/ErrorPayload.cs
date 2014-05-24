namespace RElmah.Domain
{
    public class ErrorPayload
    {
        public ErrorPayload(string sourceId, ErrorDetail detail)
        {
            SourceId = sourceId;
            Detail = detail;
        }

        public string SourceId { get; private set; }

        public ErrorDetail Detail { get; private set; }
    }
}
