using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Handlers
{
    public static class HttpHandler
    {
        private static readonly HttpClient httpClient = new();

        public static async Task<T> GetAsync<T>(Uri uri)
        {
           try
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = uri,
                    Headers =
                {
                    {"Authorization", $"JWT {AuthHelper.GetJwt()}" }
                }
                };

                var response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadFromJsonAsync<T>();

                return content;
            }
            catch(Exception ex)
            {
                throw new ($"Unknown error occurred during the get operation: {ex.Message}", ex);
            }
        }
    }
}
