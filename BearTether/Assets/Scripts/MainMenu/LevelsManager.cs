using System;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    [NonSerialized] public Level[] levels = new Level[10];

    private void Start()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == null)
            {
                levels[i] = new Level(i, 0);
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}
