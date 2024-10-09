using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public static class PathFindingSystem
{
    public static PathfindingComponent Pathfinding;

    public static void FindAllPaths(Entity Origin)
    {
        if ((Origin.HasComponent<SelectedMarker>() && !Origin.HasComponent<InMotionMarker>()) ||
            (GameManager.entities.Count(t => t.HasComponent<NeedToUpdatePathfindingMarker>()) > 0 &&
             Origin.HasComponent<PathInformationComponent>())) 
        {
            //if(Origin.GetComponent<PathInformationComponent>().paths.Count == 0 || GameManager.entities.Count(t => t.HasComponent<NeedToUpdatePathfindingMarker>()) > 0)
            //{

            //}

            foreach (var entity in
                     GameManager.entities.Where(entity => entity.HasComponent<PathfindingComponent>()))
            {
                Pathfinding = entity.GetComponent<PathfindingComponent>();
                break;
            }

            Pathfinding.grid = new PathNode[Pathfinding.Width, Pathfinding.Height];
            for (var x = 0; x < Pathfinding.Width; x++)
            {
                for (var y = 0; y < Pathfinding.Height; y++)
                {
                    Pathfinding.grid[x, y] = new PathNode(Pathfinding.grid, x, y);
                }
            }
            ObstacleUpdateSystem.Update();

            var AllowedPaths = new List<List<PathNode>>();

            var position = Origin.GetComponent<PositionComponent>();
            var movement = Origin.GetComponent<MovementInformationComponent>();

            for (var x = position.X - movement.MaxMoveDistance; x <= position.X + movement.MaxMoveDistance; x++)
            {
                for (var y = position.Y - movement.MaxMoveDistance; y <= position.Y + movement.MaxMoveDistance; y++)
                {
                    if (IsWalkable(x, y))
                    {
                        var path = FindPath(position.X, position.Y, x, y);
                        if (path != null && path.Count <= movement.MaxMoveDistance)
                        {
                            if (path.Count >= 1) AllowedPaths.Add(path);
                        }
                    }
                }
            }

            Origin.GetComponent<PathInformationComponent>().paths = AllowedPaths;

            if (Origin.HasComponent<SelectedMarker>() && !Origin.HasComponent<InMotionMarker>() && Origin.HasComponent<PlayerTurnMarker>())
            {
                var paths = Origin.GetComponent<PathInformationComponent>().paths;
                foreach (var path in paths)
                {
                    foreach (var pathNode in path)
                    {
                        int2 cords = new int2(pathNode.x, pathNode.y);
                        if (!GridDisplaySystem.GridDisplay.RenderTasks.ContainsKey(cords))
                            GridDisplaySystem.GridDisplay.RenderTasks.Add(cords, Color.green);
                    }
                }
            }

            if (Origin.HasComponent<SelectedMarker>() && !Origin.HasComponent<InMotionMarker>() && Origin.HasComponent<AIComponent>())
            {
                var paths = Origin.GetComponent<PathInformationComponent>().paths;
                Debug.Log(Origin.GetComponent<PositionComponent>().ToString() + " " +
                          Origin.GetComponent<GameObjectComponent>().gameObject.transform.position);
                foreach (var path in paths)
                {
                    foreach (var pathNode in path)
                    {
                        int2 cords = new int2(pathNode.x, pathNode.y);
                        if (!GridDisplaySystem.GridDisplay.RenderTasks.ContainsKey(cords))
                            GridDisplaySystem.GridDisplay.RenderTasks.Add(cords, Color.yellow);
                    }
                }
            }
        }
        
    }

    public static List<PathNode> FindPath(Entity Origin, Entity Target)
    {
        PositionComponent p1 = Origin.GetComponent<PositionComponent>();
        PositionComponent p2 = Target.GetComponent<PositionComponent>();
        return FindPath(p1.X, p1.Y, p2.X, p2.Y);
    }

    private static List<PathNode> FindPath(int x1, int y1, int x2, int y2) 
    {
        PathNode startNode = Pathfinding.grid[x1, y1];
        PathNode endNode = Pathfinding.grid[x2, y2];

        Pathfinding.openList = new List<PathNode>() { startNode };
        Pathfinding.closedList = new List<PathNode>();

        for (int x = 0; x < Pathfinding.grid.GetLength(0); x++)
        {
            for (int y = 0; y < Pathfinding.grid.GetLength(1); y++)
            {
                PathNode pathNode = Pathfinding.grid[x, y];
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.CameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();

        while (Pathfinding.openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(Pathfinding.openList);
            if (currentNode == endNode)
                return CalculatePath(endNode);

            Pathfinding.openList.Remove(currentNode);
            Pathfinding.closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighbourList(currentNode).Where(neighbourNode => !Pathfinding.closedList.Contains(neighbourNode)))
            {
                if (!neighbourNode.isWalkable)
                {
                    Pathfinding.closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.CameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!Pathfinding.openList.Contains(neighbourNode))
                    {
                        Pathfinding.openList.Add(neighbourNode);
                    }
                }
            }
        }

        return null;
    }

    private static List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0)
        {
            neighbourList.Add(Pathfinding.grid[currentNode.x - 1, currentNode.y]);

            if (currentNode.y - 1 >= 0)
                neighbourList.Add(Pathfinding.grid[currentNode.x - 1, currentNode.y - 1]);

            if (currentNode.y + 1 < Pathfinding.grid.GetLength(1))
                neighbourList.Add(Pathfinding.grid[currentNode.x - 1, currentNode.y + 1]);
        }

        if (currentNode.x + 1 < Pathfinding.grid.GetLength(0))
        {
            neighbourList.Add(Pathfinding.grid[currentNode.x + 1, currentNode.y]);

            if (currentNode.y - 1 >= 0)
                neighbourList.Add(Pathfinding.grid[currentNode.x + 1, currentNode.y - 1]);

            if (currentNode.y + 1 < Pathfinding.grid.GetLength(1))
                neighbourList.Add(Pathfinding.grid[currentNode.x + 1, currentNode.y + 1]);
        }

        if (currentNode.y - 1 >= 0)
            neighbourList.Add(Pathfinding.grid[currentNode.x, currentNode.y - 1]);
        if (currentNode.y + 1 < Pathfinding.grid.GetLength(1))
            neighbourList.Add(Pathfinding.grid[currentNode.x, currentNode.y + 1]);

        return neighbourList;
    }

    private static List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.CameFromNode != null)
        {
            path.Add(currentNode.CameFromNode);
            currentNode = currentNode.CameFromNode;
        }

        path.Reverse();
        return path;
    }

    private static int CalculateDistance(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return PathfindingComponent.MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + PathfindingComponent.MOVE_STRAIGHT_COST * remaining;
    }

    private static PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    public static void UpdateObstacles(List<Entity> obstacles)
    {
        if(Pathfinding != null)
        {
            for (int i = 0; i < Pathfinding.grid.GetLength(0); i++)
            {
                for (int j = 0; j < Pathfinding.grid.GetLength(1); j++)
                {
                    if (!Pathfinding.grid[i, j].isWalkable)
                        Pathfinding.grid[i, j].isWalkable = true;
                }
            }

            foreach (var obstacle in obstacles)
            {
                Pathfinding.grid[obstacle.GetComponent<PositionComponent>().X,
                    obstacle.GetComponent<PositionComponent>().Y].isWalkable = false;
            }
            GameManager.entities[0].AddComponent(new NeedToUpdatePathfindingMarker());
        }
    }

    public static bool IsWalkable(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Pathfinding.grid.GetLength(0) && y < Pathfinding.grid.GetLength(1))
            return Pathfinding.grid[x, y].isWalkable;
        return false;
    }
}
