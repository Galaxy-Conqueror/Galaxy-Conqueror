namespace Galaxy.Conqueror.API.Models.Database;

public class ResourceExtractor
{
    public int Id { get; set; }
    public int PlanetId { get; set; }
    public int Level { get; set; } = 1;
    public int ResourceGen { get; set; }
    public int UpgradedResourceGen { get; set; }
    public int UpgradeCost { get; set; }
}