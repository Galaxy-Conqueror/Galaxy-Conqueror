namespace Galaxy.Conqueror.API.Models;
public class SpaceshipResponse
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string? Design { get; set; }
    public string? Description { get; set; }
    public int Level { get; set; }
    public int CurrentFuel { get; set; }
    public int CurrentHealth { get; set; }
    public int ResourceReserve { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Damage { get; set; }
    public int MaxHealth { get; set; }
    public int MaxResources { get; set; }
    public int MaxFuel { get; set; }
    public int? PlanetResourceReserve { get; set; }
}