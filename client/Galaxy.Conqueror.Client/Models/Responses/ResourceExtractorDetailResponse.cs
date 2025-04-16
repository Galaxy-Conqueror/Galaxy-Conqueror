namespace Galaxy.Conqueror.API.Models.Responses;

public class ResourceExtractorDetailResponse
{
    public int Id { get; set; }
    public int PlanetId { get; set; }
    public int Level { get; set; }
    public int ResourceGen {  get; set; }
    public int UpgradedResourceGen {  get; set; }
    public int UpgradeCost { get; set; }
}
