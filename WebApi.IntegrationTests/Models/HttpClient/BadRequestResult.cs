using System.Collections;

namespace WebApi.IntegrationTests.Models.HttpClient
{
    public class BadRequestResult : ExceptionResult
    {
        public string Code { get; set; }
        public IDictionary Data { get; set; }

        public BadRequestResult()
        {
        }

        public BadRequestResult(string traceIdentifier, string code, string message, IDictionary data) : base(traceIdentifier, message)
        {
            Code = code;
            Data = data;
        }
    }
}
