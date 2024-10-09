using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AbilityControllerSystem
{
    public static void Update(Entity Origin)
    {
        if(Origin.HasComponent<AbilityReadyToUseMarker>())
        {
            if (Origin.HasComponent<AIComponent>())
            {
                var ai = Origin.GetComponent<AIComponent>();
                ai.BestAbility.Execute(ai.Target, Origin);
                Origin.RemoveComponent<AbilityReadyToUseMarker>();
                Origin.RemoveComponent<IsActiveMarker>();
                Origin.RemoveComponent<SelectedMarker>();
            }
            else if(/*GameManager.entities.Any(e => e.HasComponent<SelectedMarker>()) || */Origin.HasComponent<SelectedMarker>())
            {
                GridDisplaySystem.Clear();
                if (GameManager.selectedTarget != null)
                {
                    
                    var abilities = Origin.GetComponent<AbilityComponent>();
                    abilities.abilities[GameManager.abilityIndex].Execute(GameManager.selectedTarget.GetComponent<PositionComponent>(), Origin);
                    Origin.RemoveComponent<AbilityReadyToUseMarker>();
                    Origin.RemoveComponent<IsActiveMarker>();
                    Origin.RemoveComponent<SelectedMarker>();
                    GameManager.selectedTarget = null;
                }
            }
        }
    }
}
