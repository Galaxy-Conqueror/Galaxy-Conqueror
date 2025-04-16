namespace Galaxy.Conqueror.API.Models.Responses;

// Response for token exchange google auth
public class TokenResponse
{
    public string access_token { get; set; }
    public string id_token { get; set; }
}

