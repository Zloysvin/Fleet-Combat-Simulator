using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using CodeMonkey.Utils;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //public static Grid Grid = new Grid();
    //public Pathfinding Pathfinding;
    //public static List<Entity2> Entities = new List<Entity2>();
    public static GameManager Instance;

    public bool ShowDebugGrid;

    public static int abilityIndex = 0;

    public static bool IsPlayerTurn;
    public static bool IsGamePaused;

    public GameObject prePauseMenu;
    public static GameObject PauseMenu;

    public GameObject preWinScreen;
    public static GameObject WinScreen;

    public GameObject preLossScreen;
    public static GameObject LossScreen;

    public List<GameObject> preAllyPrefabs;
    public static List<GameObject> AllyPrefabs;

    public List<GameObject> preEnemyPrefabs;
    public static List<GameObject> EnemyPrefabs;

    public static List<Entity> Allies;
    public static List<Entity> Enemies;

    public static List<Entity> Targets;
    public static Entity selectedTarget;
    public static Color SelectionColor;

    public TMP_Text preName;
    public TMP_Text preType;

    public TMP_Text preHP;
    public TMP_Text preAP;

    public TMP_Text preAbility1;
    public TMP_Text preAbility1Cost;

    public TMP_Text preAbility2;
    public TMP_Text preAbility2Cost;

    public List<TMP_Text> preLog;

    public static TMP_Text Name;
    public static TMP_Text Type;

    public static TMP_Text HP;
    public static TMP_Text AP;

    public static TMP_Text Ability1;
    public static TMP_Text Ability1Cost;    
    
    public static TMP_Text Ability2;
    public static TMP_Text Ability2Cost;

    public static List<TMP_Text> Log;

    public SpriteRenderer PreGridPrefab;
    [SerializeField] public static SpriteRenderer GridPrefab;

    [SerializeField] private int GridWidth;
    [SerializeField] private int GridHeight;
    
    public static List<Entity> entities;

    void Start()
    {
        Name = preName;
        Type = preType;
        HP = preHP;
        AP = preAP;
        Ability1 = preAbility1;
        Ability2 = preAbility2;
        Ability1Cost = preAbility1Cost;
        Ability2Cost = preAbility2Cost;

        WinScreen = preWinScreen;
        LossScreen = preLossScreen;

        AllyPrefabs = preAllyPrefabs;
        EnemyPrefabs = preEnemyPrefabs;

        Log = preLog;

        GridPrefab = PreGridPrefab;
        PauseMenu = prePauseMenu;

        IsPlayerTurn = true;
        IsGamePaused = false;

        entities = new List<Entity>();
        LogSystem.logs = null;

        Entity manager = new Entity();
        manager.AddComponent(new GridComponent(GridWidth, GridHeight, 1f, Vector3.zero, ShowDebugGrid));
        manager.AddComponent(new PathfindingComponent(GridWidth, GridHeight));
        entities.Add(manager);

        GridDisplaySystem.GridDisplay = null;

        GridSystem.Update(manager);
    }

    void Update()
    {
        PauseGameSystem.Update();
        TargetSelectionSystem.Update(Targets);
        WinTrackerSystem.Check();

        if(!IsGamePaused)
        {
            GridDisplaySystem.Render();
            SelectionSystem.Select();
            ChangeUnitSystem.Change();
            for (int i = 0; i < entities.Count; i++)
            {
                if(IsGamePaused)
                    break;
                HealthSystem.Update(entities[i]);
                UIUpdateSystem.Update(entities[i]);
                ObstacleUpdateSystem.Update(entities[i]);
                PathFindingSystem.FindAllPaths(entities[i]);
                AISystem.Update(entities[i]);
                MoveSystem.Move(entities[i]);
                AbilityControllerSystem.Update(entities[i]);
            }

            if (entities[0].HasComponent<NeedToUpdatePathfindingMarker>())
                entities[0].RemoveComponent<NeedToUpdatePathfindingMarker>();
        }
    }

    public static Object SpawnObject(Object target, int x, int y)
    {
        return Instantiate(target, new Vector3(x + 0.5f, y + 0.5f), Quaternion.identity);
    }

    public static GameObject SpawnAllyShips(int x, int y, string type)
    {
        GameObject Obj;
        switch (type)
        {
            case "Battleship":
                Obj = Instantiate(AllyPrefabs[0], new Vector3(x + 0.5f, y + 0.5f, -0.1f), Quaternion.identity);
                break;
            case "Cruiser":
                Obj = Instantiate(AllyPrefabs[1], new Vector3(x + 0.5f, y + 0.5f, -0.1f), Quaternion.identity);
                break;
            case "Destroyer":
                Obj = Instantiate(AllyPrefabs[2], new Vector3(x + 0.5f, y + 0.5f, -0.1f), Quaternion.identity);
                break;
            case "Frigate":
                Obj = Instantiate(AllyPrefabs[3], new Vector3(x + 0.5f, y + 0.5f, -0.1f), Quaternion.identity);
                break;
            default:
                Obj = null;
                break;
        }

        return Obj;
    }

    public static GameObject SpawnEnemyShips(int x, int y, string type)
    {
        GameObject Obj;
        switch (type)
        {
            case "Battleship":
                Obj = Instantiate(EnemyPrefabs[0], new Vector3(x + 0.5f, y + 0.5f, -0.1f), Quaternion.identity);
                break;
            case "Cruiser":
                Obj = Instantiate(EnemyPrefabs[1], new Vector3(x + 0.5f, y + 0.5f, -0.1f), Quaternion.identity);
                break;
            case "Destroyer":
                Obj = Instantiate(EnemyPrefabs[2], new Vector3(x + 0.5f, y + 0.5f, -0.1f), Quaternion.identity);
                break;
            case "Frigate":
                Obj = Instantiate(EnemyPrefabs[3], new Vector3(x + 0.5f, y + 0.5f, -0.1f), Quaternion.identity);
                break;
            default:
                Obj = null;
                break;
        }

        return Obj;
    }

    public void ButtonOffenseClick()
    {
        if (!IsGamePaused)
        {
            IsGamePaused = true;
            Targets = new List<Entity>();

            Targets.Clear();

            Targets = entities.Where(e => e.HasComponent<AIComponent>()).ToList();

            SelectionColor = Color.red;

            GridDisplaySystem.Clear();

            foreach (var target in Targets)
            {
                GridDisplaySystem.GridDisplay.RenderTasks.Add(new int2(target.GetComponent<PositionComponent>().X,
                    target.GetComponent<PositionComponent>().Y), SelectionColor);
            }

            Entity selected = entities.FirstOrDefault(e => e.HasComponent<SelectedMarker>());
            if (selected.HasComponent<MovedMarker>())
                abilityIndex = 0;
            else
                abilityIndex = 1;

            GridDisplaySystem.Render();
        }
        else
        {
            IsGamePaused = false;

            if (entities.Any(e => e.HasComponent<IsActiveMarker>()))
            {
                var entity = entities.Where(e => e.HasComponent<IsActiveMarker>()).ToList()[0];
                var paths = entity.GetComponent<PathInformationComponent>().paths;
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

            GridDisplaySystem.Clear();
        }
    }

    public void ButtonDefenseClick()
    {
        if (!IsGamePaused)
        {
            IsGamePaused = true;
            Targets = new List<Entity>();

            Targets.Clear();

            Targets = entities.Where(e => e.HasComponent<SelectedMarker>()).ToList();

            SelectionColor = Color.blue;

            GridDisplaySystem.Clear();

            foreach (var target in Targets)
            {
                GridDisplaySystem.GridDisplay.RenderTasks.Add(new int2(target.GetComponent<PositionComponent>().X,
                    target.GetComponent<PositionComponent>().Y), SelectionColor);
            }

            Entity selected = entities.FirstOrDefault(e => e.HasComponent<SelectedMarker>());
            if (selected.HasComponent<MovedMarker>())
                abilityIndex = 3;
            else
                abilityIndex = 2;

            GridDisplaySystem.Render();
        }
        else
        {
            IsGamePaused = false;

            if (entities.Any(e => e.HasComponent<IsActiveMarker>()))
            {
                var entity = entities.Where(e => e.HasComponent<IsActiveMarker>()).ToList()[0];
                var paths = entity.GetComponent<PathInformationComponent>().paths;
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

            GridDisplaySystem.Clear();
        }
    }
}
