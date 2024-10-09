using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public static class UIUpdateSystem
{
    public static void Update(Entity Origin)
    {
        if (Origin.HasComponent<SelectedMarker>() && Origin.HasComponent<PlayerTurnMarker>())
        {
            var shipInfo = Origin.GetComponent<ShipInformationComponent>();
            var health = Origin.GetComponent<HealthComponent>();
            var abilities = Origin.GetComponent<AbilityComponent>().abilities;

            Name.text = shipInfo.Name;
            Type.text = shipInfo.Type;

            HP.text = $"HP: {health.HP}/{health.MaxHealth}";

            if (Origin.HasComponent<MovedMarker>())
            {
                AP.text = "AP: 1/2";
                Ability1.text = abilities[0].Name;
                Ability1Cost.text = "1 AP";
                Ability2.text = abilities[3].Name;
                Ability2Cost.text = "1 AP";
            }
            else
            {
                AP.text = "AP: 2/2";

                Ability1.text = abilities[1].Name;
                Ability1Cost.text = "2 AP";
                Ability2.text = abilities[2].Name;
                Ability2Cost.text = "2 AP";
            }
        }
    }
}
