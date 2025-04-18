namespace Galaxy.Conqueror.API.Models.Responses;
public class BattleResponse
{
    public int PlanetResourceReserve { get; set; }
    public int TurretHealth { get; set; }
    public int TurretDamage { get; set; }
    public int TurretLevel { get; set; }
    public int SpaceshipMaxResources { get; set; }
    public int SpaceshipResourceReserve { get; set; }
    public int SpaceshipHealth { get; set; }
    public int SpaceshipDamage { get; set; }
    public string SpaceshipDesign { get; set; } = string.Empty;
    public int SpaceshipLevel { get; set; }
}
