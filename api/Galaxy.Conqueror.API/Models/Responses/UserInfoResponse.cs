namespace Galaxy.Conqueror.API.Models;

// Response from google auth to get user details
public class UserInfoResponse
{
    public string sub { get; set; }
    public string email { get; set; }
}
