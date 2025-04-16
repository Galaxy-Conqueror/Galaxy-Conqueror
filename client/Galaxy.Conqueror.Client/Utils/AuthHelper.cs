using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Galaxy.Conqueror.Client.Models;

namespace Galaxy.Conqueror.Client.Utils
{
    public static class AuthHelper
    {
        private const string CLIENT_ID =
            "281509747189-8hu7g0egsp28dv5q7mhfgksu802mo03v.apps.googleusercontent.com";
        private const string REDIRECT_URI = "http://localhost:9090/callback/";
        private static string jwtToken = "";

        public static Guid UserId { get; set; }

        public static string GetJwt()
        {
            return jwtToken;
        }

        public static async Task Authenticate()
        {
            string authUrl =
                "https://accounts.google.com/o/oauth2/auth?client_id="
                + CLIENT_ID
                + "&redirect_uri="
                + Uri.EscapeDataString(REDIRECT_URI)
                + "&response_type=code"
                + "&scope="
                + Uri.EscapeDataString("openid email profile")
                + "&access_type=offline";

            try
            {
                Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });
            }
            catch (Exception)
            {
                Console.WriteLine("Error opening browser. Go to this URL to login: \n" + authUrl);
            }

            bool newUser = await GetJwtTokenAsync();
            if (newUser)
            {
                await SetUsername();
                await SetPlanetName();
            }
            return;
        }

        private static async Task<bool> GetJwtTokenAsync()
        {
            bool success = false;
            bool newUser = false;
            var callbackServer = new HttpListener();
            callbackServer.Prefixes.Add(REDIRECT_URI);
            callbackServer.Start();
            var context = await callbackServer.GetContextAsync();
            try
            {
                if (context?.Request?.Url != null)
                {
                    string callbackResponse = context.Request.Url.Query;

                    var authCodeMatcher = Regex.Match(callbackResponse, "code=([^&]*)");

                    if (authCodeMatcher.Success)
                    {
                        string encodedCode = authCodeMatcher.Groups[1].Value;
                        string authCode = Uri.UnescapeDataString(encodedCode);

                        if (!string.IsNullOrEmpty(authCode))
                        {
                            OutputHelper.DebugPrint("AUTH CODE: " + authCode);

                            var requestBody = new { authCode = authCode };
                            var response = await RequestHelper.UnauthedPostRequestAsync(
                                "/api/auth/login",
                                "",
                                JsonSerializer.Serialize(requestBody)
                            );

                            var responseContent = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                };
                                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(
                                    responseContent,
                                    options
                                );

                                OutputHelper.DebugPrint("JWT: " + loginResponse?.JWT);

                                jwtToken = loginResponse != null ? loginResponse.JWT : "";
                                success = jwtToken != "";

                                UserId = loginResponse?.User?.Id ?? Guid.NewGuid();

                                if (loginResponse?.User?.Username?.Length == 0)
                                {
                                    newUser = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                string browserResponse;
                if (success)
                {
                    browserResponse = "Login successful, you may now close this window";
                    Console.WriteLine("Login successful");
                }
                else
                {
                    browserResponse =
                        "Login unsuccessful, you may now close this window and try again";
                    Console.WriteLine("Login unsuccessful, try again");
                }
                var responseOutput = context.Response;
                responseOutput.StatusCode = 200;
                byte[] output = Encoding.UTF8.GetBytes(browserResponse);
                responseOutput.ContentLength64 = output.Length;
                await responseOutput.OutputStream.WriteAsync(output);
                responseOutput.OutputStream.Close();
                callbackServer.Stop();
            }
            return newUser;
        }

        private static async Task SetUsername()
        {
            while (true)
            {
                Console.Write("Enter username: ");
                var username = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(username) || username.Length > 255)
                {
                    Console.WriteLine("Username should be between 1 and 255 characters in length, try again.\n");
                    continue;
                }

                var request = new { Username = username };
                var response = await RequestHelper.PutRequestAsync(
                    "/api/user",
                    "",
                    JsonSerializer.Serialize(request)
                );

                if (response.IsSuccessStatusCode)
                    break;

                Console.WriteLine("Failed to set username, try again.\n");
            }
        }

        public static async Task SetPlanetName()
        {
            while (true)
            {
                Console.Write("Enter planet name: ");
                var planetName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(planetName) || planetName.Length > 255)
                {
                    Console.WriteLine("Planet name should be between 1 and 255 characters in length, try again.\n");
                    continue;
                }

                var request = new { PlanetName = planetName };
                var response = await RequestHelper.PutRequestAsync(
                    "/api/planet",
                    "",
                    JsonSerializer.Serialize(request)
                );

                if (response.IsSuccessStatusCode)
                    break;

                Console.WriteLine("Failed to set planet name, try again.\n");
            }
        }

    }
}
