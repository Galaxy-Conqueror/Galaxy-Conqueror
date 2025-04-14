using Galaxy.Conqueror.Client.Utils;

public class Turret
{
    public int X { get; set; }
    public int Y { get; set; }
    public Glyph Glyph { get; private set; }

    public Turret(int x, int y, Glyph glyph)
    {
        X = x;
        Y = y;
        Glyph = glyph;
    }

    public Vector2I GetPosition()
    {
        return new Vector2I(X, Y);
    }
}