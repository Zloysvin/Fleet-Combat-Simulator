using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionComponent : IComponent
{
    public int X, Y;

    public PositionComponent(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        PositionComponent other = obj as PositionComponent;

        if(other.X == X && other.Y == Y) return true;
        return false;
    }

    public override string ToString()
    {
        return $"X:{X} Y:{Y}";
    }
}
