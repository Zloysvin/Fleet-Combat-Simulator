using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    public GameObject SettingsMenu;

    public TMP_Text PointsText;

    public int[] Points = new int[5];

    public Button Simulate;

    public TMP_Dropdown ship1;
    public TMP_Dropdown ship2;
    public TMP_Dropdown ship3;
    public TMP_Dropdown ship4;
    public TMP_Dropdown ship5;

    public TMP_Text description1;
    public TMP_Text description2;
    public TMP_Text description3;
    public TMP_Text description4;
    public TMP_Text description5;

    public TMP_Text composition;

    public List<Entity> AllyShips;
    public List<Entity> EnemyShips;

    public int[,] EnemyShipsPreset;

    private string[] names = new[]
    {
        "Adept",
        "Adroit",
        "Agile",
        "Swift",
        "Quicksilver",
        "Reckless", "Acrimonious",
        "Antagonistic",
        "Bellicose",
        "Belligerent",
        "Pernicious",
        "Vicious",
        "Vehement", "Last Rains of Sestramor",
        "Eyes Like Nuclear Fire",
        "Mercurial Scythe",
        "Fated Death",
        "Wrath",
        "Kelos",
        "Heart of Gold",
        "Nostos",
        "Nostromo",
        "Laserhawk",
        "Pequod",
        "Caledon",
        "Saratan",
        "Heart of Darkness",
        "Outcast",
        "Golden Arrow",
        "Bright Spark",
        "Rose Celestial",
        "Chance",
        "There And Back Again",
        "Nightwitch",
        "Roman Road",
        "Scudding Swiftly",
        "Hot Pursuit",
        "Star Blazer",
        "Lonely Rover",
        "Perdido",
        "Sorceror",
        "Faraway And Gone",
        "Scrivener",
        "Scrimshaw",
        "Spanish Dancer",
        "Boreas",
        "Notos",
        "Auster",
        "Skiron",
        "Prosperous Gale",
        "Lotus-eater",
        "Nomad",
        "Pariah",
        "Babylonian",
        "Porphyra",
        "Ambergris",
        "Ultraviolet Sunrise",
        "Indigo",
        "Singularian",
        "Amber",
        "Enigmata",
        "Waking Dream",
        "Bright Tide",
        "Third Questioning",
        "Home Away",
        "His Eyes Black",
        "At Long Last",
        "All Of Our Hopes",
        "Malfeasant",
        "Crowning Glory",
        "Ionian Regent",
        "Iridescence",
        "Lucky Dragon",
        "Fortuitous Voyager",
        "Highest High",
        "Golden Glow",
        "Aquilius",
        "Moskva-Kassiopeya",
        "Sabled Sun",
        "Alizarin",
        "Omnichrome",
        "Immaculate Vessel",
        "Voidclipper",
        "Dawn Trekker",
        "Miraculux",
        "Voyageur",
        "Three Sheets To The Wind",
        "Nameless",
        "Arkady Sent Me",
        "Drifter",
        "Solo",
        "Aeon Raptor",
        "Unity of Multitude",
        "Hyperparabolic",
        "Lone Star",
        "Give It A Go",
        "Long Shot",
        "Hello And Goodbye",
        "Come And Gone",
        "Go On Take A Good Look",
        "Not A Peep Of This",
        "Don't Mind If I Do",
        "Limit One Per Customer",
        "This Time For Real",
        "Let's Give It Another Go",
        "Aim To Please",
        "Amoral Quandry",
        "Frequently Modulated",
        "Impartial Eclipse",
        "You Found Me Now What",
        "A Lot To Live Down"
    };

    // 0 - BS 10
    // 1 - C 7
    // 2 - D 5
    // 3 - F 3
    // 4 - empty

    void Start()
    {
        Points = new[] {0, 0, 0, 0, 0};
        EnemyShipsPreset = new[,] { {0, 1, 3, 4, 4}, {1, 2, 3, 3, 4}, /*unfair -> */ {0, 0, 1, 3, 3}, { 1, 1, 1, 3, 3 }, { 2, 2, 2, 2, 2 } };
        EnemyShips = new List<Entity>();
        AllyShips = new List<Entity>();

        int EnemyShipsIndex = (int)(UnityEngine.Random.value * EnemyShipsPreset.GetLength(0));
        for (int i = 0; i < EnemyShipsPreset.GetLength(1); i++)
        {
            if (EnemyShipsPreset[EnemyShipsIndex, i] != 4)
            {
                EnemyShips.Add(GetShips(EnemyShipsPreset[EnemyShipsIndex, i], false));
            }
        }

        foreach (var enemyShip in EnemyShips)
        {
            composition.text += enemyShip.GetComponent<ShipInformationComponent>().Type + " ";
        }
    }

    public void OpenSettings()
    {
        SettingsMenu.SetActive(true);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void GetShip1()
    {
        string option = ship1.options[ship1.value].text;
        description1.gameObject.SetActive(true);
        description1.text = option;
        Points[0] = GetPoints(ship1.value);
        UpdatePointsText();
    }

    public void GetShip2()
    {
        string option = ship2.options[ship2.value].text;
        description2.gameObject.SetActive(true);
        description2.text = option;
        Points[1] = GetPoints(ship2.value);
        UpdatePointsText();
    }

    public void GetShip3()
    {
        string option = ship3.options[ship3.value].text;
        description3.gameObject.SetActive(true);
        description3.text = option;
        Points[2] = GetPoints(ship3.value);
        UpdatePointsText();
    }

    public void GetShip4()
    {
        string option = ship4.options[ship4.value].text;
        description4.gameObject.SetActive(true);
        description4.text = option;
        Points[3] = GetPoints(ship4.value);
        UpdatePointsText();
    }

    public void GetShip5()
    {
        string option = ship5.options[ship5.value].text;
        description5.gameObject.SetActive(true);
        description5.text = option;
        Points[4] = GetPoints(ship5.value);
        UpdatePointsText();
    }

    private int GetPoints(int index)
    {
        switch (index)
        {
            case 0: return 10;
            case 1: return 7;
            case 2: return 5;
            case 3: return 3;
            case 4: return 0;
        }

        return 0;
    }

    private int GetIndex(int value)
    {
        switch (value)
        {
            case 10: return 0;
            case 7: return 1;
            case 5: return 2;
            case 3: return 3;
            case 0: return 4;
        }

        return 0;
    }

    private Entity GetShips(int index, bool isAlly)
    {
        Entity newShip = new Entity();
        switch (index)
        {
            case 0:
                newShip.AddComponent(new ShipInformationComponent(GenerateName(), "Battleship")); ;
                newShip.AddComponent(new MovementInformationComponent(3));
                newShip.AddComponent(new PathInformationComponent(new List<List<PathNode>>()));
                newShip.AddComponent(new HealthComponent(25, 25));
                newShip.AddComponent(new AbilityComponent(new List<IAbility>()
                {
                    new SmallCannon("Large Cannon", 5, 60, 1), new SmallCannon("Double Large Cannon", 10, 50, 2),
                    new EvasionAbility("Full Burn", 10, 0, 2), new EvasionAbility("Half Burn", 5, 0, 1)
                }));
                newShip.AddComponent(new IsUnitMarker());
                newShip.AddComponent(new ImpassableMarker());
                break;

            case 1:
                newShip.AddComponent(new ShipInformationComponent(GenerateName(), "Cruiser")); ;
                newShip.AddComponent(new MovementInformationComponent(4));
                newShip.AddComponent(new PathInformationComponent(new List<List<PathNode>>()));
                newShip.AddComponent(new HealthComponent(15, 15));
                newShip.AddComponent(new AbilityComponent(new List<IAbility>()
                {
                    new SmallCannon("Medium Cannon", 3, 60, 1), new SmallCannon("Double Medium Cannon", 6, 50, 2),
                    new EvasionAbility("Full Burn", 10, 0, 2), new EvasionAbility("Half Burn", 5, 0, 1)
                }));
                newShip.AddComponent(new IsUnitMarker());
                newShip.AddComponent(new ImpassableMarker());
                break;

            case 2:
                newShip.AddComponent(new ShipInformationComponent(GenerateName(), "Destroyer")); ;
                newShip.AddComponent(new MovementInformationComponent(5));
                newShip.AddComponent(new PathInformationComponent(new List<List<PathNode>>()));
                newShip.AddComponent(new HealthComponent(10, 10));
                newShip.AddComponent(new AbilityComponent(new List<IAbility>()
                {
                    new SmallCannon("Medium Cannon", 3, 60, 1), new SmallCannon("Double Medium Cannon", 6, 50, 2),
                    new EvasionAbility("Full Burn", 10, 0, 2), new EvasionAbility("Half Burn", 5, 0, 1)
                }));
                newShip.AddComponent(new IsUnitMarker());
                newShip.AddComponent(new ImpassableMarker());

                break;

            case 3:
                newShip.AddComponent(new ShipInformationComponent(GenerateName(), "Frigate")); ;
                newShip.AddComponent(new MovementInformationComponent(8));
                newShip.AddComponent(new PathInformationComponent(new List<List<PathNode>>()));
                newShip.AddComponent(new HealthComponent(10, 10));
                newShip.AddComponent(new AbilityComponent(new List<IAbility>()
                {
                    new SmallCannon("Small Cannon", 2, 60, 1), new SmallCannon("Double Medium Cannon", 4, 50, 2),
                    new EvasionAbility("Full Burn", 10, 0, 2), new EvasionAbility("Half Burn", 5, 0, 1)
                }));
                newShip.AddComponent(new IsUnitMarker());
                newShip.AddComponent(new ImpassableMarker());

                break;
        }
        if (isAlly)
        {
            newShip.AddComponent(new PlayerTurnMarker());
            newShip.AddComponent(new IsActiveMarker());
        }
        else
            newShip.AddComponent(new AIComponent());

        return newShip;
    }

    private string GenerateName()
    {
        return names[UnityEngine.Random.Range(0, names.Length)];
    }

    private void UpdatePointsText()
    {
        int Sum = 0;
        for (int i = 0; i < Points.Length; i++)
        {
            Sum += Points[i];
        }

        if (Sum == 0 || 20 - Sum < 0)
            Simulate.interactable = false;
        else
            Simulate.interactable = true;

        PointsText.text = $"Points Left: {20 - Sum}";
    }

    public void LoadGame()
    {
        for (int i = 0; i < 5; i++)
        {
            if (Points[i] != 0)
            {
                AllyShips.Add(GetShips(GetIndex(Points[i]), true));
            }
        }

        GameManager.Allies = AllyShips;
        GameManager.Enemies = EnemyShips;

        SceneManager.LoadScene("SampleScene");
    }
}
