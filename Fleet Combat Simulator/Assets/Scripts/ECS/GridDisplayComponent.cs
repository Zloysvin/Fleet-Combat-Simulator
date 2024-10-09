using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridDisplayComponent : IComponent
{
    public SpriteRenderer GridPrefab;
    public SpriteRenderer[,] GridRenderer;

    public Dictionary<int2, Color> RenderTasks;

    public GridDisplayComponent(SpriteRenderer gridPrefab)
    {
        GridPrefab = gridPrefab;
    }
}
