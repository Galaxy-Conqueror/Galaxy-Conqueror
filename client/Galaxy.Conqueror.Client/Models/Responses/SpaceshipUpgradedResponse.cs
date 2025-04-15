namespace Galaxy.Conqueror.API.Models.Responses;
public class SpaceshipUpgradedResponse
{
    public int Level { get; set; }
    public int Damage { get; set; }
    public int MaxHealth { get; set; }
    public int MaxResources { get; set; }
    public int MaxFuel { get; set; }
    public int PlanetResourceReserve { get; set; }
}