using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static GridSystem;

public class GridDisplaySystem
{
    public static GridDisplayComponent GridDisplay;

    public static void Render()
    {
        if (GridDisplay == null)
        {
            GridDisplay = new GridDisplayComponent(GameManager.GridPrefab);
            GridDisplay.GridRenderer = new SpriteRenderer[GridSystem.Grid.width, GridSystem.Grid.height];
            for (int x = 0; x < GridDisplay.GridRenderer.GetLength(0); x++)
            {
                for (int y = 0; y < GridDisplay.GridRenderer.GetLength(1); y++)
                {
                    GridDisplay.GridRenderer[x,y] = GameManager.SpawnObject(GridDisplay.GridPrefab, x,y) as SpriteRenderer;
                }
            }
        }

        if(GridDisplay.RenderTasks == null)
            GridDisplay.RenderTasks = new Dictionary<int2, Color>();

        foreach (var task in GridDisplay.RenderTasks)
        {
            GridDisplay.GridRenderer[task.Key.x, task.Key.y].color = task.Value;
        }

        GridDisplay.RenderTasks.Clear();
    }

    public static void Clear()
    {
        GridDisplay.RenderTasks.Clear();
        for (int x = 0; x < GridDisplay.GridRenderer.GetLength(0); x++)
        {
            for (int y = 0; y < GridDisplay.GridRenderer.GetLength(1); y++)
            {
                GridDisplay.GridRenderer[x, y].color = new Color(43 / 255f, 126 / 255f, 26 / 255f, 1f);
            }
        }
    }
}
