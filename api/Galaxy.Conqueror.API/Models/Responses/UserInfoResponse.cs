namespace Galaxy.Conqueror.API.Models.Responses;

// Response from google auth to get user details
public class UserInfoResponse
{
    public string sub { get; set; }
    public string email { get; set; }
}
