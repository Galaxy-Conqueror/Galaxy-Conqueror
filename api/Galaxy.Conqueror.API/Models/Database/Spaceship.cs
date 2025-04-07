namespace Galaxy.Conqueror.API.Models.Database;

public class Spaceship
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int WeoponLevel { get; set; }
    public float FuelLevel { get; set; }
    public int FuelCapacity { get; set; }
    public float ResourceCapacity { get; set; }
    public float? X { get; set; }
    public float? Y { get; set; }

}
