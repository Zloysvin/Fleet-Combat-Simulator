using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WinTrackerSystem
{
    public static void Check()
    {
        bool AllyDestroyed = true;
        bool EnemyDestroyed = true;

        foreach (var entity in GameManager.entities)
        {
            if (AllyDestroyed && entity.HasComponent<PlayerTurnMarker>())
                AllyDestroyed = false;

            if (EnemyDestroyed && entity.HasComponent<AIComponent>())
                EnemyDestroyed = false;
        }

        if (AllyDestroyed)
        {
            GameManager.LossScreen.SetActive(true);
            GameManager.IsGamePaused = true;
        }
        else if (EnemyDestroyed)
        {
            GameManager.WinScreen.SetActive(true);
            GameManager.IsGamePaused = true;
        }
    }
}
