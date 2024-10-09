using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using static GameManager;
using static UnityEngine.UI.Image;

public static class ChangeUnitSystem
{
    public static void Change()
    {
        bool areActiveUnits = false;
        bool isASelectedUnit = false;
        bool readyToUseAbilities = false;

        Entity selected = new Entity();

        foreach (var entity in entities.Where(entity => entity.HasComponent<IsUnitMarker>()))
        {
            if (!areActiveUnits)
            {
                areActiveUnits = entity.HasComponent<IsActiveMarker>();
            }

            if (!isASelectedUnit)
            {
                isASelectedUnit = entity.HasComponent<SelectedMarker>();
                if (isASelectedUnit)
                    selected = entity;
            }

            if (!readyToUseAbilities)
            {
                readyToUseAbilities = entity.HasComponent<AbilityReadyToUseMarker>();
            }
        }

        if (!areActiveUnits && !isASelectedUnit && !(IsPlayerTurn && readyToUseAbilities))
        {
            GridDisplaySystem.Clear();
            IsPlayerTurn = !IsPlayerTurn;

            foreach (var entity in entities.Where(entity => entity.HasComponent<IsUnitMarker>()))
            {
                if (entity.HasComponent<PlayerTurnMarker>() == IsPlayerTurn)
                {
                    entity.AddComponent(new IsActiveMarker());
                    if(entity.HasComponent<EvasionBoostComponent>())
                        entity.RemoveComponent<EvasionBoostComponent>();

                    if (entity.HasComponent<MovedMarker>())
                        entity.RemoveComponent<MovedMarker>();
                }
            }

            entities.Where(entity => entity.HasComponent<IsUnitMarker>())
                .Where(entity2 => entity2.HasComponent<PlayerTurnMarker>() == IsPlayerTurn).ToList()[0]
                .AddComponent(new SelectedMarker());
        }
        else if (areActiveUnits && !isASelectedUnit && !(IsPlayerTurn && readyToUseAbilities))
        {
            foreach (var entity in entities.Where(entity => entity.HasComponent<IsActiveMarker>()))
            {
                GridDisplaySystem.Clear();
                entity.AddComponent(new SelectedMarker());
                break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && IsPlayerTurn)
        {
            GridDisplaySystem.Clear();
            var activeEntities = entities.Where(entity => entity.HasComponent<IsActiveMarker>()).ToList();

            if(isASelectedUnit)
            {
                int index = activeEntities.IndexOf(selected);
                int nextIndex = index + 1;
                if (nextIndex >= activeEntities.Count)
                {
                    nextIndex -= activeEntities.Count;
                }

                activeEntities[index].RemoveComponent<SelectedMarker>();
                activeEntities[nextIndex].AddComponent(new SelectedMarker());
                Debug.Log(activeEntities[nextIndex].GetComponent<GameObjectComponent>().gameObject.name + " Is now active!");
            }
            else
            {
                foreach (var activeEntity in activeEntities)
                {
                    if (!activeEntity.HasComponent<AbilityReadyToUseMarker>())
                    {
                        activeEntity.AddComponent(new SelectedMarker());
                    }
                }
            }
        }
    }
}
