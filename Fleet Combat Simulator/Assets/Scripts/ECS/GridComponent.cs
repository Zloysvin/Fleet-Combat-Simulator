using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridComponent : IComponent
{
    public int width;
    public int height;
    public float cellSize;
    public Vector3 originPosition;
    public Entity[,] gridArray;
    public TextMesh[,] debugTextArray;

    public const int MOVE_STRAIGHT_COST = 10;
    public const int MOVE_DIAGONAL_COST = 14;

    public bool ShowDebug;

    public GridComponent(int width, int height, float cellSize, Vector3 originPosition, bool showDebug)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        ShowDebug = showDebug;
    }
}
