using System;
using System.IO;
using UnityEngine;

public class Options
{
    private readonly string _path = Application.persistentDataPath + "/SettingsData.json";

    public OptionsData optionsData = new OptionsData();

    private static Options _instance;

    // Implementation of the Singleton pattern
    public static Options Instance
    {
        get
        {
            _instance ??= new Options();

            return _instance;
        }
    }

    // Class with fields for storage
    [Serializable]
    public class OptionsData
    {
        public float musicVolume = .7f;
        public float soundVolume = .7f;
        public Vector2 buttonLeftPosition = new Vector2(300, 180);
        public Vector2 buttonRightPosition = new Vector2(550, 180);
        public Vector2 buttonJumpPosition = new Vector2(-300, 300);
    }

    public void Save()
    {
        if (!Directory.Exists(Application.persistentDataPath))
            Directory.CreateDirectory(Application.persistentDataPath);

        File.WriteAllText(_path, JsonUtility.ToJson(optionsData));
    }

    public void Load()
    {
        if (!File.Exists(_path))
            return;

        optionsData = JsonUtility.FromJson<OptionsData>(File.ReadAllText(_path));
    }
}