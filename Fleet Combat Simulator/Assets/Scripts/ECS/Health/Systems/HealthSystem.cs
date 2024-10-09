using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class HealthSystem
{
    public static void Update(Entity Origin)
    {
        if (Origin.HasComponent<HealthComponent>() && Origin.HasComponent<DealDamageComponent>())
        {
            var health = Origin.GetComponent<HealthComponent>();
            health.HP -= Origin.GetComponent<DealDamageComponent>().Damage;
            Origin.RemoveComponent<DealDamageComponent>();

            if (health.HP <= 0)
            {
                var position = Origin.GetComponent<PositionComponent>();
                var emptyEnt = new Entity();
                var info = Origin.GetComponent<ShipInformationComponent>();
                emptyEnt.AddComponent(new EmptyMarker());
                GridSystem.SetValue(position.X, position.Y, emptyEnt);
                GameManager.entities.Remove(Origin);
                LogSystem.Update($"{info.Type} {info.Name} has been destroyed");
                Origin.GetComponent<GameObjectComponent>().gameObject.SetActive(false);
            }
        }
    }
}
