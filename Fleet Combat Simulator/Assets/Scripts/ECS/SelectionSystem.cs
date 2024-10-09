using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public static class SelectionSystem
{
    public static void Select()
    {
        var selectedList = GameManager.entities.Where(n => n.HasComponent<SelectedMarker>()).ToList();

        if(selectedList.Count > 0)
        {
            Entity selected = selectedList[0];

            var cords = selected.GetComponent<PositionComponent>();

            GridDisplaySystem.GridDisplay.RenderTasks.Add(new int2(cords.X, cords.Y), Color.yellow);
        }
    }
}
