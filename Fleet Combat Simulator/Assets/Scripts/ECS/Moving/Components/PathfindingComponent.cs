using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingComponent : IComponent
{
    public const int MOVE_STRAIGHT_COST = 10;
    public const int MOVE_DIAGONAL_COST = 14;

    public PathNode[,] grid;
    public List<PathNode> openList;
    public List<PathNode> closedList;

    public int Width;
    public int Height;

    public PathfindingComponent(int width, int height)
    {
        this.openList = new List<PathNode>();
        this.closedList = new List<PathNode>();
        Width = width;
        Height = height;
    }
}
