namespace Galaxy.Conqueror.API.Models.Database;

public class Spaceship
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string? Design { get; set; }
    public string? Description { get; set; }
    public int Level { get; set; } = 1;
    public int CurrentFuel { get; set; }
    public int CurrentHealth { get; set; }
    public int ResourceReserve { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}