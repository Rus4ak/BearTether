using System;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    [NonSerialized] public Level[] levels = new Level[10];
    [NonSerialized] public Level[] multiplayerLevels = new Level[10];

    private void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LevelsManager");

        if (objs.Length > 1)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        InitializeLevels();
    }

    public void InitializeLevels()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == null)
                levels[i] = new Level(i, 0);
        }

        for (int i = 0; i < multiplayerLevels.Length; i++)
        {
            if (multiplayerLevels[i] == null)
                multiplayerLevels[i] = new Level(i, 0);
        }
    }
}
