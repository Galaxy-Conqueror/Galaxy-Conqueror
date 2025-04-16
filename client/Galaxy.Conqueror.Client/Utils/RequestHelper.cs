using System.Net;
using System.Text;
using System.Web;

namespace Galaxy.Conqueror.Client.Utils
{
    public static class RequestHelper
    {
        private static readonly string BASE_URL = "http://13.246.15.180:8080";
        private static readonly HttpClient CLIENT = new();

        private static string BuildUrl(string path, string queryParams)
        {
            string url = string.IsNullOrWhiteSpace(queryParams)
                ? $"{BASE_URL}{path}"
                : $"{BASE_URL}{path}?{queryParams}";

            OutputHelper.DebugPrint($"API CALL MADE: {url}");
            return url;
        }

        private static string EncodedQueryParam(string key, string value)
        {
            try
            {
                return string.IsNullOrEmpty(value)
                    ? ""
                    : $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}";
            }
            catch
            {
                return "";
            }
        }

        public static string AddQueryParam(string queryParams, string key, string value)
        {
            var encodedParam = EncodedQueryParam(key, value);
            if (string.IsNullOrEmpty(encodedParam))
                return queryParams;

            if (string.IsNullOrWhiteSpace(queryParams))
                return encodedParam;

            return $"{queryParams}&{encodedParam}";
        }

        public static async Task<HttpResponseMessage> GetRequestAsync(
            string path,
            string? queryParams = ""
        )
        {
            OutputHelper.DebugPrint("GET REQUEST");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, BuildUrl(path, queryParams));
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        AuthHelper.GetJwt()
                    );
                return await CLIENT.SendAsync(request);
            }
            catch
            {
                OutputHelper.DebugPrint("GET REQUEST FAILED");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Get request failed"),
                };
            }
        }

        public static async Task<HttpResponseMessage> PostRequestAsync(
            string path,
            string? queryParams = "",
            string? body = ""
        )
        {
            OutputHelper.DebugPrint("POST REQUEST");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(path, queryParams));
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        AuthHelper.GetJwt()
                    );
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                return await CLIENT.SendAsync(request);
            }
            catch
            {
                OutputHelper.DebugPrint("POST REQUEST FAILED");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Post request failed"),
                };
            }
        }

        public static async Task<HttpResponseMessage> UnauthedPostRequestAsync(
            string path,
            string queryParams,
            string body
        )
        {
            OutputHelper.DebugPrint("UNAUTHED POST REQUEST");
            try
            {
                var postData = new StringContent(body, Encoding.UTF8, "application/json");
                return await CLIENT.PostAsync(BuildUrl(path, queryParams), postData);
            }
            catch
            {
                OutputHelper.DebugPrint("UNAUTHED POST REQUEST FAILED");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Request failed"),
                };
            }
        }

        public static async Task<HttpResponseMessage> PutRequestAsync(
            string path,
            string? queryParams = "",
            string? body = ""
        )
        {
            OutputHelper.DebugPrint("PUT REQUEST");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, BuildUrl(path, queryParams));
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        AuthHelper.GetJwt()
                    );
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                return await CLIENT.SendAsync(request);
            }
            catch
            {
                OutputHelper.DebugPrint("PUT REQUEST FAILED");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Put request failed"),
                };
            }
        }

        public static async Task<HttpResponseMessage> DeleteRequestAsync(
            string path,
            string queryParams
        )
        {
            OutputHelper.DebugPrint("DELETE REQUEST");
            try
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    BuildUrl(path, queryParams)
                );
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        AuthHelper.GetJwt()
                    );
                return await CLIENT.SendAsync(request);
            }
            catch
            {
                OutputHelper.DebugPrint("DELETE REQUEST FAILED");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Delete request failed"),
                };
            }
        }
    }
}
