using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static Progress;

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

        byte[] data = SerializeToBytes(optionsData);

        if (PlayGamesManager.isAuthenticated)
        {
            PlayGamesManager.SaveToCloud(data, "options");
        }
    }

    public void Load()
    {
        if (!File.Exists(_path))
            return;

        optionsData = JsonUtility.FromJson<OptionsData>(File.ReadAllText(_path));

        SoundVolume.Instance.volume = optionsData.soundVolume;
    }

    public void LoadFromCLoud()
    {
        optionsData = DeserializeFromBytes(PlayGamesManager.LoadFromCloud("options"));

        SoundVolume.Instance.volume = optionsData.soundVolume;
    }

    private byte[] SerializeToBytes(OptionsData data)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, data);
            return memoryStream.ToArray();
        }
    }

    private OptionsData DeserializeFromBytes(byte[] bytes)
    {
        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (OptionsData)formatter.Deserialize(memoryStream);
        }
    }
}