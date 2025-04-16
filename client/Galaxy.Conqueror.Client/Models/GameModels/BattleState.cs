namespace Galaxy.Conqueror.Client.Models.GameModels;
public class BattleState
{
    public int SpaceshipHealth { get; set; }
    public int TurretHealth { get; set; }
    public float BattleDurationSeconds { get; set; }

    public BattleState(int spaceshipHealth, int turretHealth, float duration)
    {
        SpaceshipHealth = spaceshipHealth;
        TurretHealth = turretHealth;
        BattleDurationSeconds = duration;
    }
}

