using Galaxy.Conqueror.API.Models.Responses;
using Galaxy.Conqueror.Client.Utils;
using System.Xml.Linq;
namespace Galaxy.Conqueror.Client.Models.GameModels;

public class Turret : Entity
{
    public int Level { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public int Damage { get; set; }
    public int UpgradeCost { get; set; }

    public Turret()
    {
        Id = int.MaxValue;
        Level = 0;
        CurrentHealth = 0;
        MaxHealth = 0;
        Damage = 0;
        UpgradeCost = 0;
    }


    public Turret(int id, string name, Glyph glyph, Vector2I position) : base(id, name, glyph, position)
    {
        Id = id;
        Name = name;
        Glyph = glyph;
        Position = position;
    }

    public static Turret GetTurretFromServerModel(Turret serverTurret)
    {
        var turret = new Turret();

        turret.Id = serverTurret.Id;
        turret.Level = serverTurret.Level;
        turret.CurrentHealth = serverTurret.CurrentHealth;
        turret.Damage = serverTurret.Damage;
        turret.MaxHealth = serverTurret.MaxHealth;
        turret.UpgradeCost = serverTurret.UpgradeCost;

        return turret;
    }

    public Vector2I GetPosition()
    {
        return new Vector2I(Position);
    }

    public void TakeDamage(Bullet bullet)
    {
        CurrentHealth -= bullet.Damage;
    }

    public bool IsDestroyed()
    {
        return CurrentHealth <= 0;
    }
}
