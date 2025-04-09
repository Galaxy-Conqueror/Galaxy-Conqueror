namespace Galaxy.Conqueror.API.Models.Requests;

public class BattleLogRequest
{
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public int DamageToSpaceship { get; set; }
    public int DamageToTurret { get; set; }
    public int ResourcesLooted { get; set; }
}
