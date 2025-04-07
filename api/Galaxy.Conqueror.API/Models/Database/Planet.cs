namespace Galaxy.Conqueror.API.Models.Database;

public class Planet
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public float? X { get; set; }
    public float? Y { get; set; }
}

