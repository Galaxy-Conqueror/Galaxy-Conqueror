using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Galaxy.Conqueror.Client.Utils
{
    public static class AuthHelper
    {
        private const string CLIENT_ID =
            "281509747189-8hu7g0egsp28dv5q7mhfgksu802mo03v.apps.googleusercontent.com";
        private const string REDIRECT_URI = "http://localhost:9090/callback/";
        public static string jwtToken = "";

        public static string getJwt()
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

            await GetJwtTokenAsync();
            return;
        }

        public static async Task GetJwtTokenAsync()
        {
            bool success = false;
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
                        if (string.IsNullOrEmpty(authCode))
                        {
                            success = false;
                        }
                        else
                        {
                            Console.WriteLine("Auth Code: " + authCode);
                            success = true;

                            // TODO: Make POST request

                            var httpClient = new HttpClient();
                            var requestBody = new
                            {
                                authCode = authCode
                            };

                            var json = JsonSerializer.Serialize(requestBody);
                            var postData = new StringContent(json, Encoding.UTF8, "application/json");

                            var response = await httpClient.PostAsync("https://localhost:7292/api/auth/login", postData);

                            // TODO Ensure this is successful
                            // TODO Get response content and save the JWT from this content
                            var responseContent = await response.Content.ReadAsStringAsync();


                            // if (response != null && response.StatusCode == HttpStatusCode.OK)
                            // {
                            //     jwtToken = await response.Content.ReadAsStringAsync();
                            //     success = true;
                            // }
                            // else
                            // {
                            //     success = false;
                            // }
                        }
                    }
                    else
                        success = false;
                }
                else
                    success = false;
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                string browserResponse = "";
                if (success)
                {
                    browserResponse = "Login successful";
                    Console.WriteLine("Login successful");
                    Console.WriteLine("JWT Token: " + jwtToken);
                }
                else
                {
                    browserResponse = "Login unsuccessful";
                    Console.WriteLine("Login unsuccessful");
                }
                var responseOutput = context.Response;
                responseOutput.StatusCode = 200;
                byte[] output = Encoding.UTF8.GetBytes(browserResponse);
                responseOutput.ContentLength64 = output.Length;
                await responseOutput.OutputStream.WriteAsync(output);
                responseOutput.OutputStream.Close();
                callbackServer.Stop();
            }
        }
    }
}
