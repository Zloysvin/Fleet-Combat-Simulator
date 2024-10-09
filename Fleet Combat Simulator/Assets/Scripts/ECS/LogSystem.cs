using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class LogSystem
{
    public static List<TMP_Text> logs;
    public static List<string> logEntries;
    public static void Update(string message)
    {
        if (logs == null)
        {
            logs = GameManager.Log;
            logEntries = new List<string>();
            for (int i = 0; i < logs.Count; i++)
            {
                logEntries.Add("");
            }
        }

        logEntries.RemoveAt(0);
        logEntries.Add(message);

        for (int i = 0; i < logs.Count; i++)
        {
            logs[i].text = logEntries[i];
        }
    }
}
