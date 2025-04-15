using Galaxy.Conqueror.Client.Utils;
namespace Galaxy.Conqueror.Client.Models.GameModels;

public class Turret : Entity
{
    public int Level { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }

    public Turret(int id, string name, Glyph glyph, Vector2I position) : base(id, name, glyph, position)
    {
        Id = id;
        Name = name;
        Glyph = glyph;
        Position = position;
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
