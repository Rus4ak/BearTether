using System;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    [NonSerialized] public int countCompletedLevels;

    [NonSerialized] public Level[] levels = new Level[10];
    [NonSerialized] public bool isRewardedFinishMap = false;

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
            {
                levels[i] = new Level(i, 0);
            }

            if (levels[i].isCompleted)
                countCompletedLevels++;
        }
    }
}
