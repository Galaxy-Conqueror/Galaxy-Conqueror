using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Models;

// Send back user info and JWT to user on login
public class LoginResponse
{
    public required User User { get; set; }
    public required string JWT { get; set; }
}
