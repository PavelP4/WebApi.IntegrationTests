using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text;
using Xunit;
using WebApi.IntegrationTests.Common;
using System.Net.Http.Headers;

namespace WebApi.IntegrationTests.Extentions
{
    public static class HttpClientExts
    {
        public static async Task<TResponse> GetResultAsync<TResponse>(this HttpResponseMessage httpResponse)
        {
            if (typeof(TResponse) == typeof(HttpResponseMessage))
            {
                return (TResponse)Convert.ChangeType(httpResponse, typeof(TResponse));
            }

            var buffer = await httpResponse.Content.ReadAsByteArrayAsync();
            var content = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            return typeof(TResponse) == typeof(string) ?
                (TResponse)Convert.ChangeType(content, typeof(TResponse)) :
                string.IsNullOrEmpty(content) ?
                    (TResponse)Convert.ChangeType(null, typeof(TResponse)) :
                    JsonSerializer.Deserialize<TResponse>(content, GetJsonOptions());
        }

        public static async Task CheckResultAsync<TResponse>(this Task<HttpResponseMessage> httpResponseTask, Action<TResponse> checkAction)
        {
            var httpResponse = await httpResponseTask;
            await httpResponse.CheckResultAsync(checkAction);
        }

        public static async Task CheckResultAsync<TResponse>(this HttpResponseMessage httpResponse, Action<TResponse> checkAction)
        {
            await httpResponse.IsSuccessStatusCode();
            var result = await httpResponse.GetResultAsync<TResponse>();
            checkAction(result);
        }

        public static async Task<HttpResponseMessage> IsSuccessStatusCode(this HttpResponseMessage httpResponse)
        {
            await httpResponse.EnsureSuccessStatusCodeReadContent();
            return httpResponse;
        }

        public static void IsNotFound(this HttpResponseMessage httpResponse)
        {
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        public static void IsUnauthorized(this HttpResponseMessage httpResponse)
        {
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }

        public static void IsForbidden(this HttpResponseMessage httpResponse)
        {
            Assert.Equal(HttpStatusCode.Forbidden, httpResponse.StatusCode);
        }

        public static async Task IsForbidden(this HttpResponseMessage httpResponse, string checkContentPart)
        {
            Assert.Equal(HttpStatusCode.Forbidden, httpResponse.StatusCode);
            await CheckContentPart(httpResponse, checkContentPart);
        }

        public static void IsBadRequest(this HttpResponseMessage httpResponse)
        {
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        public static async Task IsBadRequest(this HttpResponseMessage httpResponse, string checkContentPart)
        {
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            await CheckContentPart(httpResponse, checkContentPart);
        }

        public static async Task IsRedirect(this HttpResponseMessage httpResponse, string checkContentPart)
        {
            Assert.Equal(HttpStatusCode.Redirect, httpResponse.StatusCode);
            await CheckContentPart(httpResponse, checkContentPart);
        }

        private static async Task CheckContentPart(HttpResponseMessage httpResponse, string checkContentPart)
        {
            if (!string.IsNullOrEmpty(checkContentPart))
            {
                var content = httpResponse.StatusCode == HttpStatusCode.Redirect ?
                    httpResponse.Headers.Location?.ToString() :
                    await httpResponse.ReadContent();

                Debug.Assert(content != null, nameof(content) + " != null");
                Assert.Contains(checkContentPart, content);
            }
        }

        public static JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new TimeSpanToStringConverter() }
            };
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClient client, string url, string token = null)
        {
            return client.SendRequest<object>(url, HttpMethod.Get, token);
        }

        public static Task<HttpResponseMessage> PostAsync<TRequest>(this HttpClient client, string url, TRequest requestData, string token = null, Stream fileContent = null)
        {
            return client.SendRequest(url, HttpMethod.Post, token, requestData, fileContent);
        }

        public static Task<HttpResponseMessage> PutAsync<TRequest>(this HttpClient client, string url, TRequest requestData, string token = null, Stream fileContent = null)
        {
            return client.SendRequest(url, HttpMethod.Put, token, requestData, fileContent);
        }

        public static Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string url, string token = null)
        {
            return client.SendRequest<object>(url, HttpMethod.Delete, token);
        }

        public static Task<HttpResponseMessage> DeleteAsync<TRequest>(this HttpClient client, string url, TRequest requestData, string token)
        {
            return client.SendRequest<object>(url, HttpMethod.Delete, token, requestData);
        }

        public static async Task<HttpResponseMessage> SendRequest<TRequest>(this HttpClient client, string url, HttpMethod method, string token = null,
            TRequest requestData = default, Stream fileContent = null)
        {
            var httpRequest = new HttpRequestMessage(method, url);

            if (!string.IsNullOrEmpty(token))
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue(Constants.Common.TestAuthenticationScheme, token);
            }

            if (fileContent != null)
            {
                var formDataValues = requestData as KeyValuePair<string, string>[];
                var formData = new MultipartFormDataContent
                {
                    {new StreamContent(fileContent), "file", "file"}
	            };

                foreach (var formDataValue in formDataValues)
                {
                    formData.Add(new StringContent(formDataValue.Value), formDataValue.Key);
                }

                httpRequest.Content = formData;
            }
            else if (requestData != null)
            {
                var data = JsonSerializer.Serialize(requestData, GetJsonOptions());

                httpRequest.Content = new StringContent(data, Encoding.UTF8, "application/json");
            }
            var httpResponse = await client.SendAsync(httpRequest);
            return httpResponse;
        }
    }
}
