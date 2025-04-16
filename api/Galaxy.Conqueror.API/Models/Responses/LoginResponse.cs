using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Models.Responses;
public class LoginResponse
{
    public required User User { get; set; }
    public required string JWT { get; set; }
}
