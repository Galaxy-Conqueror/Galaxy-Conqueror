

namespace Galaxy.Conqueror.Client.Utils;

public class Vector2I
{
    public int X { get; set; }
    public int Y { get; set; }

    public static readonly Vector2I MAX = new Vector2I(int.MaxValue, int.MaxValue);
    public static readonly Vector2I ZERO = new Vector2I(0, 0);

    public Vector2I(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2I(Vector2I vector)
    {
        X = vector.X;
        Y = vector.Y;
    }

    public static bool operator ==(Vector2I left, Vector2I right)
    {
        if (ReferenceEquals(left, null))
            return ReferenceEquals(right, null);

        return left.Equals(right);
    }

    public static bool operator !=(Vector2I left, Vector2I right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Vector2I other = (Vector2I)obj;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return (X << 16) ^ Y;
    }

    public double DistanceTo(Vector2I other)
    {
        int dx = X - other.X;
        int dy = Y - other.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
