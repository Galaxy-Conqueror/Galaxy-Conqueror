namespace Galaxy.Conqueror.API.Models.Database;

public class Planet
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public string? Design { get; set; }
    public string? Description { get; set; }
    public int ResourceReserve { get; set; } = 0;
    public int X { get; set; }
    public int Y { get; set; }
}