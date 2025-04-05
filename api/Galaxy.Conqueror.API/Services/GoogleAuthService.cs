using System.Text.Json;
using Galaxy.Conqueror.API.Models;

namespace Galaxy.Conqueror.API.Services;

public class GoogleAuthService(HttpClient httpClient, IConfiguration configuration)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _clientId = configuration["Google:ClientId"];
    private readonly string _clientSecret = configuration["Google:ClientSecret"];
    private readonly string _redirectUri = configuration["Google:RedirectUri"];

    public async Task<TokenResponse> ExchangeAuthCodeForTokens (string authCode)
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

        return JsonSerializer.Deserialize<TokenResponse>(responseContent);
    }
}
