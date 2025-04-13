namespace Galaxy.Conqueror.API.Models.Responses;

public class TurretDetailResponse
{
    public int Id { get; set; }
    public int PlanetId { get; set; }
    public int Level { get; set; }
    public int Damage { get; set; }
    public int Health { get; set; }
    public int UpgradeCost { get; set; }
    public int UpgradedDamage { get; set; }
    public int UpgradedHealth { get; set; }
}
