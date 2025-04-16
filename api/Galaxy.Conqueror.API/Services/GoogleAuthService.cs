using System.Text.Json;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Models.Responses;

namespace Galaxy.Conqueror.API.Services;

public class GoogleAuthService(
    HttpClient httpClient,
    IUserService userService,
    IConfiguration configuration, 
    IHostEnvironment env)
{
    private readonly HttpClient httpClient = httpClient;
    public async Task<LoginResponse> Login (string authCode)
    {
        var tokens = await ExchangeAuthCodeForTokens(authCode);
        var user = await GetUserInfo(tokens.access_token);
        var loginResponse = new LoginResponse() { User = user, JWT = tokens.id_token };
        return loginResponse;
    }

    private async Task<TokenResponse> ExchangeAuthCodeForTokens(string authCode)
    {
 
        string clientId, clientSecret, redirectUri;
        if (env.IsDevelopment())
        {
            clientId = configuration["Google:ClientId"] ?? "";
            clientSecret = configuration["Google:ClientSecret"] ?? "";
            redirectUri = configuration["Google:RedirectUri"] ?? "";
        } else
        {
            clientId = Environment.GetEnvironmentVariable("CLIENT_ID") ?? "";
            clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET") ?? "";
            redirectUri = Environment.GetEnvironmentVariable("REDIRECT_URI") ?? "";
        }
        var requestData = new Dictionary<string, string>
        {
            { "code", authCode },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "redirect_uri", redirectUri },
            { "grant_type", "authorization_code" }
        };

        var requestContent = new FormUrlEncodedContent(requestData);

        var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", requestContent);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Token exchange failed: {response.StatusCode}, Body: {responseContent}");
        }

        var tokens = JsonSerializer.Deserialize<TokenResponse>(responseContent);
        return tokens;
    }

    private async Task<User> GetUserInfo(string access_token)
    {
        var userInfoResponse = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={access_token}");
        var userInfoResponseContent = await userInfoResponse.Content.ReadAsStringAsync();

        var userInfo = JsonSerializer.Deserialize<UserInfoResponse>(userInfoResponseContent);

        var user = await userService.CreateUser(userInfo);
        return user;
    }
}
