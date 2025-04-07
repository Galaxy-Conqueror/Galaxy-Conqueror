namespace Galaxy.Conqueror.API.Models.Database;

public class Battle
{
    public int Id { get; set; }
    public Guid AttackerId { get; set; }
    public Guid DefenderId { get; set; }
    public int DefenderPlanetId { get; set; } // We dont need the attackers planet
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; } // For battle durations stat
    public bool? AttackerWon { get; set; } // either this or have another ref to the user: public Guid WinnerID 
    public int DamageToSpaceship { get; set; }
    public int DamageToTurret { get; set; } // Doesn't really matter as we reset it but maybe for stats
    public int ResourcesLooted { get; set; }
    public Planet? AttackerPlanet { get; set; }
    public Planet? DefenderPlanet { get; set; }
}
