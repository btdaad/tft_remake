using System;

public struct Coords
{
    public int x;
    public int y;
    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Coords lhs, Coords rhs)
    {
        return (lhs.x == rhs.x && lhs.y == rhs.y);
    }

    public static bool operator !=(Coords lhs, Coords rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals(object obj)
    {
        return obj is Coords && this == (Coords) obj;
    }

    public override int GetHashCode()
    {
        return this.GetHashCode();
    }
}