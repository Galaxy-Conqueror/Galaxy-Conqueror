namespace Galaxy.Conqueror.Client.Models.GameModels;
public class BattleResult
{
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public int DamageToSpaceship { get; set; }
    public int DamageToTurret { get; set; }
    public int ResourcesLooted { get; set; }

    public BattleResult(DateTime startedAt, DateTime endedAt, int damageToSpaceship, int damageToTurret, int resourcesLooted)
    {
        StartedAt = startedAt;
        EndedAt = endedAt;
        DamageToSpaceship = damageToSpaceship;
        DamageToTurret = damageToTurret;
        ResourcesLooted = resourcesLooted;
    }
}
