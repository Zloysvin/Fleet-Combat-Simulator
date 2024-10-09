using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PauseGameSystem
{
    public static void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.IsGamePaused)
        {
            GameManager.IsGamePaused = true;
            GameManager.PauseMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.IsGamePaused)
        {
            GameManager.IsGamePaused = false;
            GameManager.PauseMenu.SetActive(false);
        }
    }
}
