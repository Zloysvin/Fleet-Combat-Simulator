using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public static class GridSystem
{
    public static GridComponent Grid;

    public static void Update(Entity Origin)
    {
        if(Origin.HasComponent<GridComponent>() && Origin.GetComponent<GridComponent>().gridArray == null)
        {
            foreach (var entity in GameManager.entities)
            {
                if (entity.HasComponent<GridComponent>())
                {
                    Grid = entity.GetComponent<GridComponent>();
                    break;
                }
            }

            Grid.gridArray = new Entity[Grid.width, Grid.height];

            for (int x = 0; x < Grid.gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < Grid.gridArray.GetLength(1); y++)
                {
                    var emptyEnt = new Entity();
                    emptyEnt.AddComponent(new EmptyMarker());
                    SetValue(x, y, emptyEnt);
                }
            }

            var EntObj = GameObject.FindGameObjectsWithTag("Obstacle");
            if (EntObj != null)
            {
                foreach (var obj in EntObj)
                {
                    Entity newObstcl = new Entity();
                    newObstcl.AddComponent(GetXY(obj.transform.position));
                    newObstcl.AddComponent(new ObstacleMarker());
                    newObstcl.AddComponent(new ImpassableMarker());
                    newObstcl.AddComponent(new GameObjectComponent(obj));

                    GameManager.entities.Add(newObstcl);

                    SetValue(newObstcl.GetComponent<PositionComponent>().X,
                        newObstcl.GetComponent<PositionComponent>().X,
                        newObstcl);
                }
            }

            foreach (var entity in GameManager.Allies)
            {
                var position = new PositionComponent(Random.Range(0, 9), Random.Range(0, 3));
                while (!Grid.gridArray[position.X, position.Y].HasComponent<EmptyMarker>())
                {
                    position = new PositionComponent(Random.Range(0, 9), Random.Range(0, 3));
                }

                entity.AddComponent(position);
                entity.AddComponent(new GameObjectComponent(GameManager.SpawnAllyShips(position.X, position.Y,
                    entity.GetComponent<ShipInformationComponent>().Type)));

                GameManager.entities.Add(entity);
            }

            foreach (var entity in GameManager.Enemies)
            {
                var position = new PositionComponent(Random.Range(0, 9), Random.Range(7, 9));
                while (!Grid.gridArray[position.X, position.Y].HasComponent<EmptyMarker>())
                {
                    position = new PositionComponent(Random.Range(0, 9), Random.Range(0, 3));
                }

                entity.AddComponent(position);
                entity.AddComponent(new GameObjectComponent(GameManager.SpawnEnemyShips(position.X, position.Y,
                    entity.GetComponent<ShipInformationComponent>().Type)));

                GameManager.entities.Add(entity);
            }

            if (Grid.ShowDebug)
            {
                Grid.debugTextArray = new TextMesh[Grid.width, Grid.height];

                for (int x = 0; x < Grid.gridArray.GetLength(0); x++)
                {
                    for (int y = 0; y < Grid.gridArray.GetLength(1); y++)
                    {
                        Grid.debugTextArray[x, y] = UtilsClass.CreateWorldText(Grid.gridArray[x, y].ToString(), null,
                            GetWorldPosition(x, y) + new Vector3(Grid.cellSize, Grid.cellSize) * .5f, 30, Color.white,
                            TextAnchor.MiddleCenter);
                        Grid.debugTextArray[x, y].transform.localScale = Vector3.one * 0.08f;
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                    }
                }

                Debug.DrawLine(GetWorldPosition(0, Grid.height), GetWorldPosition(Grid.width, Grid.height), Color.white,
                    100f);
                Debug.DrawLine(GetWorldPosition(Grid.width, 0), GetWorldPosition(Grid.width, Grid.height), Color.white,
                    100f);
            }

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
        }
    }
    
    public static Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * Grid.cellSize + Grid.originPosition;
    }

    public static PositionComponent GetXY(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - Grid.originPosition).x / Grid.cellSize);
        int y = Mathf.FloorToInt((worldPosition - Grid.originPosition).y / Grid.cellSize);
        return new PositionComponent(x, y);
    }

    public static void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - Grid.originPosition).x / Grid.cellSize);
        y = Mathf.FloorToInt((worldPosition - Grid.originPosition).y / Grid.cellSize);
    }

    public static void SetValue(int x, int y, Entity value)
    {
        if (x >= 0 && y >= 0 && x < Grid.width && y < Grid.height)
        {
            Grid.gridArray[x, y] = value;
        }
    }

    public static void SetValue(Vector3 worldPosition, Entity value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public static Entity GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Grid.width && y < Grid.height)
        {
            return Grid.gridArray[x, y];
        }
        else
        {
            return default(Entity);
        }
    }

    public static Entity GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        Debug.Log(x + " " + y);
        return GetValue(x, y);
    }

    public static bool SwapValues(int x1, int y1, int x2, int y2)
    {
        if (Grid.gridArray[x2, y2].HasComponent<EmptyMarker>() ||
            (Grid.gridArray[x2, y2].GetComponent<PositionComponent>().X != x2 || Grid.gridArray[x2, y2].GetComponent<PositionComponent>().Y != y2))
        {
            Grid.gridArray[x2,y2] = Grid.gridArray[x1, y1];
            Grid.gridArray[x1, y1] = new Entity();
            Grid.gridArray[x1,y1].AddComponent(new EmptyMarker());

            Grid.gridArray[x2, y2].GetComponent<PositionComponent>().X = x2;
            Grid.gridArray[x2, y2].GetComponent<PositionComponent>().Y = y2;

            return true;
        }

        return false;
    }

    public static int CalculateDistance(int x1, int y1, int x2, int y2)
    {
        int xDistance = Mathf.Abs(x1 - x2);
        int yDistance = Mathf.Abs(y1 - y2);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return GridComponent.MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + GridComponent.MOVE_STRAIGHT_COST * remaining;
    }

    public static void SetCustomDebugText(int x, int y, string text)
    {
        if (Grid.ShowDebug)
            Grid.debugTextArray[x, y].text = text;
    }
}
