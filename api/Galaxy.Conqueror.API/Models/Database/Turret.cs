namespace Galaxy.Conqueror.API.Models.Database;

public class Turret
{
    public int Id { get; set; }
    public int PlanetId { get; set; }
    public float Health { get; set; }
    public int Level { get; set; } // The level of upgrade
    public float Defence { get; set; }

}
