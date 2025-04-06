namespace Galaxy.Conqueror.Client.Models;

internal class LoginResponse
{
    public required User User { get; set; }
    public required string JWT { get; set; }
}
