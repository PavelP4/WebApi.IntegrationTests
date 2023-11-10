using System.Collections;
using System.Net;
using System.Text.Json;
using WebApi.IntegrationTests.Exceptions.HttpClient;
using WebApi.IntegrationTests.Models.HttpClient;

namespace WebApi.IntegrationTests.Extentions
{
    public static class HttpResponseMessageExts
    {
        public static async Task EnsureSuccessStatusCodeReadContent(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await ReadContent(response);

            throw new HttpRequestException(
                $"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}). Content: {content}",
                GetInnerException(response.StatusCode, content));
        }

        public static async Task<string> ReadContent(this HttpResponseMessage response)
        {
            var content = string.Empty;
            if (response.Content != null)
            {
                content = await response.Content.ReadAsStringAsync();
                response.Content.Dispose();
            }

            return content.Substring(0, Math.Min(content.Length, 2000));
        }

        private static Exception GetInnerException(HttpStatusCode statusCode, string content)
        {
            if (statusCode == HttpStatusCode.BadRequest)
            {
                var data = JsonSerializer.Deserialize<BadRequestResult>(content, HttpClientExts.GetJsonOptions());
                if (!string.IsNullOrEmpty(data.Code) || !string.IsNullOrEmpty(data.Message))
                {
                    var exception = new ErrorCodeException(data.Code, data.Message);
                    if (data.Data != null)
                    {
                        foreach (DictionaryEntry entry in data.Data)
                        {
                            //if(entry.Value is JsonArray)
                            exception.Data.Add(entry.Key, ToObject(entry.Value));
                        }
                    }
                    return exception;
                }
            }

            return null;
        }

        private static object ToObject(object ob)
        {
            if (ob is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.Array:
                        return jsonElement.EnumerateArray().Select(x => ToObject(x)).ToArray();
                    case JsonValueKind.String:
                        return jsonElement.GetString();
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        return jsonElement.GetBoolean();
                    case JsonValueKind.Null:
                        return null;
                    case JsonValueKind.Number:
                        return jsonElement.GetDecimal();
                }
            }

            return ob;
        }
    }
}
