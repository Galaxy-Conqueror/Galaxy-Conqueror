using System.Text.Json;
using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Services;

public class GoogleAuthService(
    HttpClient httpClient,
    UserService userService,
    IConfiguration configuration)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _clientId = configuration["Google:ClientId"];
    private readonly string _clientSecret = configuration["Google:ClientSecret"];
    private readonly string _redirectUri = configuration["Google:RedirectUri"];

    public async Task<LoginResponse> Login (string authCode)
    {
        var tokens = await ExchangeAuthCodeForTokens(authCode);
        var user = await GetUserInfo(tokens.access_token);
        var loginResponse = new LoginResponse() { User = user, JWT = tokens.id_token };
        return loginResponse;
    }

    private async Task<TokenResponse> ExchangeAuthCodeForTokens(string authCode)
    {
        var requestData = new Dictionary<string, string>
        {
            { "code", authCode },
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "redirect_uri", _redirectUri },
            { "grant_type", "authorization_code" }
        };

        var requestContent = new FormUrlEncodedContent(requestData);

        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", requestContent);

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
        var userInfoResponse = await _httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={access_token}");
        var userInfoResponseContent = await userInfoResponse.Content.ReadAsStringAsync();

        var userInfo = JsonSerializer.Deserialize<UserInfoResponse>(userInfoResponseContent);

        var user = await userService.CreateUser(userInfo);
        return user;
    }
}
