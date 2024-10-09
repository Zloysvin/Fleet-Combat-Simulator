using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObstacleUpdateSystem
{
    public static void Update(Entity Origin)
    {
        if(Origin.HasComponent<PositionComponent>() && !Origin.HasComponent<InMotionMarker>())
        {
            var position = Origin.GetComponent<PositionComponent>();
            if (!Equals(position,
                    GridSystem.GetXY(Origin.GetComponent<GameObjectComponent>().gameObject.transform.position)))
            {
                var actualPosition =
                    GridSystem.GetXY(Origin.GetComponent<GameObjectComponent>().gameObject.transform.position);
                GridSystem.SwapValues(position.X, position.Y, actualPosition.X, actualPosition.Y);
            }
        }

        PathFindingSystem.UpdateObstacles(GameManager.entities.Where(t => t.HasComponent<ImpassableMarker>())
            .ToList());
    }

    public static void Update()
    {
        PathFindingSystem.UpdateObstacles(GameManager.entities.Where(t => t.HasComponent<ImpassableMarker>())
            .ToList());

    }
}
