using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.UI.Image;

public static class TargetSelectionSystem 
{
    public static void Update(List<Entity> Targets)
    {
        if (GameManager.IsGamePaused && !GameManager.PauseMenu.activeSelf && Targets != null && Targets.Count != 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var supposedTarget = GridSystem.GetValue(UtilsClass.GetMouseWorldPosition());
                var newPosition = GridSystem.GetXY(UtilsClass.GetMouseWorldPosition());

                foreach (var target in Targets)
                {
                    if (Equals(target.GetComponent<PositionComponent>(), newPosition))
                    {
                        GameManager.selectedTarget = target;
                        var origin = GameManager.entities.Where(e => e.HasComponent<SelectedMarker>()).ToList()[0];
                        origin.AddComponent(new AbilityReadyToUseMarker());
                        GameManager.IsGamePaused = false;
                        break;
                    }
                }
            }
        }
    }
}
