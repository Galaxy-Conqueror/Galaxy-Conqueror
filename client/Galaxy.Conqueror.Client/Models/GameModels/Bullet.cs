using Galaxy.Conqueror.Client.Utils;
namespace Battle;

public class Bullet
{
    public int X { get; private set; }
    public float Y { get; private set; }
    public float Speed { get; private set; }
    public Glyph Glyph { get; private set; }


    public Bullet(int x, float y, float speed, Glyph glyph)
    {
        X = x;
        Y = y;
        Speed = speed;
        Glyph = glyph;
    }

    public void Update(float deltaTime)
    {
        float movement = Speed * deltaTime;
        Y += movement;
    }

    public Vector2I GetPosition()
    {
        return new Vector2I(X, (int)Math.Round(Y));
    }
}