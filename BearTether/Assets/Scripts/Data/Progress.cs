using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Progress
{
    private readonly string _path = Application.persistentDataPath + "/ProgressData.json";

    public ProgressData progressData = new ProgressData();

    private static Progress _instance;

    // Implementation of the Singleton pattern
    public static Progress Instance
    {
        get
        {
            _instance ??= new Progress();

            return _instance;
        }
    }

    // Class with fields for storage
    [Serializable]
    public class ProgressData
    {
        public int bank;
        public Level[] levels = new Level[10];
    }

    public void Save()
    {
        if (!Directory.Exists(Application.persistentDataPath))
            Directory.CreateDirectory(Application.persistentDataPath);

        // Serialize data to bytes
        byte[] data = SerializeToBytes(progressData);

        File.WriteAllBytes(_path, data);
    }

    public void Load()
    {
        if (!File.Exists(_path))
            return;
        
        // Deserialize data from bytes
        byte[] bytes = File.ReadAllBytes(_path);
        progressData = DeserializeFromBytes(bytes);

        LevelsManager levelsManager = GameObject.FindWithTag("LevelsManager").GetComponent<LevelsManager>();
        levelsManager.levels = Instance.progressData.levels;
        levelsManager.InitializeLevels();
        Bank.Instance.Coins = Instance.progressData.bank;
    }

    private byte[] SerializeToBytes(ProgressData data)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, data);
            return memoryStream.ToArray();
        }
    }

    private ProgressData DeserializeFromBytes(byte[] bytes)
    {
        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (ProgressData)formatter.Deserialize(memoryStream);
        }
    }
}
