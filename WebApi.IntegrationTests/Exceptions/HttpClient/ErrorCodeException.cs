using System.Collections;
using System.Net;
using WebApi.IntegrationTests.Extentions;

namespace WebApi.IntegrationTests.Exceptions.HttpClient
{
    public class ErrorCodeException : Exception
    {
        public HttpStatusCode? HttpStatusCode { get; }
        public string Code { get; }

        public ErrorCodeException(Enum code, string additionalMessage = null, HttpStatusCode? httpStatusCode = null)
            : base(code.GetMessage() + (string.IsNullOrEmpty(additionalMessage) ? null : additionalMessage))
        {
            HttpStatusCode = httpStatusCode;
            Code = code.GetCode();
        }


        public ErrorCodeException(string code, string message) : base(message)
        {
            Code = code;
        }

        public ErrorCodeException(HttpStatusCode httpStatusCode, string code, string message) : base(message)
        {
            HttpStatusCode = httpStatusCode;
            Code = code;
        }

        public ErrorCodeException(string code, string message, IDictionary data) : this(code, message)
        {
            foreach (DictionaryEntry entry in data)
            {
                Data.Add(entry.Key, entry.Value);
            }
        }

        public ErrorCodeException(Enum code, IDictionary data) : this(code.GetCode(), code.GetMessage(), data)
        {
        }

        public static void ThrowIfTrue(bool condition, string code, string message)
        {
            if (condition)
            {
                throw new ErrorCodeException(code, message);
            }
        }

        public static void ThrowIfTrue(bool condition, HttpStatusCode httpStatusCode, string code, string message)
        {
            if (condition)
            {
                throw new ErrorCodeException(httpStatusCode, code, message);
            }
        }

        public static void ThrowIfTrue(bool condition, Enum value)
        {
            if (condition)
            {
                throw new ErrorCodeException(value);
            }
        }

        public static void ThrowIfFalse(bool condition, Enum value)
        {
            ThrowIfTrue(!condition, value);
        }

    }
}
