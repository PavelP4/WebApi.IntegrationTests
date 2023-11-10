namespace WebApi.IntegrationTests.Models.HttpClient
{
    public class ExceptionResult
    {
        public string TraceIdentifier { get; set; }
        public string Message { get; set; }

        // ReSharper disable once UnusedMember.Global used for deserialization
        public ExceptionResult()
        {

        }

        public ExceptionResult(string traceIdentifier, string message)
        {
            TraceIdentifier = traceIdentifier;
            Message = message;
        }
    }
}
