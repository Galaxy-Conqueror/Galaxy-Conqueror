namespace Galaxy.Conqueror.API.Models.Database;
public class User
{
    public Guid Id { get; set; }
    public string? GoogleId { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
}
