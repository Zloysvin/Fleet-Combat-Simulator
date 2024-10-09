using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public static class MoveSystem
{
    private static int waypointIndex;
    public static void Move(Entity Origin)
    {
        if (Origin.HasComponent<SelectedMarker>() && !Origin.HasComponent<InMotionMarker>() && !Origin.HasComponent<AbilityReadyToUseMarker>())
        {
            var pathfinding = Origin.GetComponent<PathInformationComponent>();
            bool isMouseOverUI = EventSystem.current.IsPointerOverGameObject();

            if (Input.GetMouseButtonDown(0) && Origin.HasComponent<PlayerTurnMarker>() && !isMouseOverUI)
            {
                var newPosition = GridSystem.GetXY(UtilsClass.GetMouseWorldPosition());
                foreach (var path in pathfinding.paths)
                {
                    if (path[^1].x == newPosition.X && path[^1].y == newPosition.Y)
                    {
                        pathfinding.selectedPath = path;
                        Origin.AddComponent(new InMotionMarker());
                        break;
                    }
                }
            }
            else if (Origin.HasComponent<AIComponent>() && Origin.GetComponent<AIComponent>().MoveCoords.Count != 0)
            {
                pathfinding.selectedPath = Origin.GetComponent<AIComponent>().MoveCoords;
                Debug.Log($"Path Length: {pathfinding.selectedPath.Count}");
                Origin.AddComponent(new InMotionMarker());
            }
        }

        if (Origin.HasComponent<InMotionMarker>())
        {
            var originTransform = Origin.GetComponent<GameObjectComponent>().gameObject.transform;
            var path = Origin.GetComponent<PathInformationComponent>().selectedPath;
            var movement = Origin.GetComponent<MovementInformationComponent>();

            originTransform.position = new Vector3(originTransform.position.x, originTransform.position.y, -0.1f);
            if (waypointIndex < path.Count)
            {
                Vector3 target = new Vector3(path[waypointIndex].x + 0.5f, path[waypointIndex].y + 0.5f);
                float step = movement.MoveSpeed * Time.deltaTime;
                originTransform.position = Vector3.MoveTowards(originTransform.position, target, step);

                if (Vector3.Distance(originTransform.position, target) < 0.1f)
                {
                    waypointIndex++;
                }
            }
            else
            {
                originTransform.position =
                    new Vector3(path[waypointIndex - 1].x + 0.5f, path[waypointIndex - 1].y + 0.5f, -0.1f);

                Origin.RemoveComponent<InMotionMarker>();
                //Origin.RemoveComponent<SelectedMarker>();
                //Origin.RemoveComponent<IsActiveMarker>();

                Origin.AddComponent(new AbilityReadyToUseMarker());
                Origin.AddComponent(new MovedMarker());

                //Debug.Log(
                //    $"OBJ:{Origin.GetComponent<GameObjectComponent>().gameObject.name} {Origin.GetComponent<PathInformationComponent>().selectedPath[0].ToString()} {Origin.GetComponent<PathInformationComponent>().selectedPath[^1].ToString()}");

                Origin.GetComponent<PathInformationComponent>().paths = new List<List<PathNode>>();

                if(Origin.HasComponent<AIComponent>())
                {
                    var aiComponent = Origin.GetComponent<AIComponent>();
                    aiComponent.MaxAbilityScore = 0;
                    aiComponent.MoveCoords = new List<PathNode>();
                }

                //КОСТИЛЬ!!!

                for (int i = 0; i < GridSystem.Grid.width; i++)
                {
                    for (int j = 0; j < GridSystem.Grid.height; j++)
                    {
                        foreach (var entity in GameManager.entities.Where(t => t.HasComponent<IsUnitMarker>()))
                        {
                            if (entity.GetComponent<PositionComponent>().X == i &&
                                entity.GetComponent<PositionComponent>().Y == j)
                            {
                                if (entity != GridSystem.GetValue(i, j))
                                {
                                    GridSystem.SetValue(i, j, entity);
                                }
                            }
                        }
                    }

                }

                GridSystem.SwapValues(Origin.GetComponent<PositionComponent>().X,
                    Origin.GetComponent<PositionComponent>().Y, path[waypointIndex - 1].x, path[waypointIndex - 1].y);
                waypointIndex = 0;
                GameManager.entities[0].AddComponent(new NeedToUpdatePathfindingMarker());

                //Debug.Log(Origin.GetComponent<PositionComponent>().ToString() + " " +
                //          Origin.GetComponent<GameObjectComponent>().gameObject.transform.position);

                //Origin.GetComponent<PathInformationComponent>().paths = new List<List<PathNode>>();
                //if(Origin.HasComponent<AIComponent>())
                //Debug.Log("paths left:"+ Origin.GetComponent<PathInformationComponent>().paths.Count);
            }
        }
    }
}
