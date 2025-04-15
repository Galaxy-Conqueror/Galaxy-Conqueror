namespace Galaxy.Conqueror.API.Models.Old;

public class User
{
    public Guid Id { get; set; }
    public required string GoogleId { get; set; }
    public required string Email { get; set; }
    public string? Username { get; set; }
}
