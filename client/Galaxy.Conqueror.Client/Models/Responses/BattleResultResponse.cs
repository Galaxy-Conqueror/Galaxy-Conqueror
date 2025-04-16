
public class BattleResultResponse
{
    public int Id { get; set; }
    public int AttackerSpaceshipId { get; set; }
    public int DefenderPlanetId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public bool AttackerWon { get; set; }
    public int DamageToSpaceship { get; set; }
    public int DamageToTurret { get; set; }
    public int ResourcesLooted { get; set; }
}