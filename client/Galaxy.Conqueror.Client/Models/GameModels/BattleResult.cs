namespace Battle;
public struct BattleResult
{
    public int SpaceshipHealth { get; set; }
    public int TurretHealth { get; set; }
    public string WinnerName { get; set; }
    public float BattleDurationSeconds { get; set; }

    public BattleResult(int spaceshipHealth, int turretHealth, string winnerName, float duration)
    {
        SpaceshipHealth = spaceshipHealth;
        TurretHealth = turretHealth;
        WinnerName = winnerName;
        BattleDurationSeconds = duration;
    }
}
