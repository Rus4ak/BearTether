using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        public Level[] levels = new Level[30];
        public Level[] levelsHardcore = new Level[30];
        public Level[] multiplayerLevels = new Level[30];
        public Level[] multiplayerLevelsHardcore = new Level[30];
        public List<MapData> maps = new List<MapData>();
        public List<MapData> mapsMultiplayer = new List<MapData>();
        public bool isShownReview;
        //public bool isBoughtHardcore;
    }

    public void Save()
    {
        if (!Directory.Exists(Application.persistentDataPath))
            Directory.CreateDirectory(Application.persistentDataPath);

        // Serialize data to bytes
        byte[] data = SerializeToBytes(progressData);

        File.WriteAllBytes(_path, data);

        if (PlayGamesManager.isAuthenticated)
        {
            PlayGamesManager.SaveToCloud(data, "progress");
        }
    }

    public void Load()
    {
        if (File.Exists(_path))
        {
            byte[] bytes = File.ReadAllBytes(_path);
            DeserializeAndLoad(bytes);
        }
    }

    public void DeserializeAndLoad(byte[] bytes)
    {
        // Deserialize data from bytes
        progressData = DeserializeFromBytes(bytes);

        LevelsManager levelsManager = GameObject.FindWithTag("LevelsManager").GetComponent<LevelsManager>();
        levelsManager.levels = Instance.progressData.levels;
        levelsManager.levelsHardcore = Instance.progressData.levelsHardcore;
        levelsManager.multiplayerLevels = Instance.progressData.multiplayerLevels;
        levelsManager.multiplayerLevelsHardcore = Instance.progressData.multiplayerLevelsHardcore;
        levelsManager.InitializeLevels();
        Bank.Instance.Coins = Instance.progressData.bank;
        MapsData.maps = Instance.progressData.maps;
        MapsData.mapsMultiplayer = Instance.progressData.mapsMultiplayer;
        IARManager.isShownReview = Instance.progressData.isShownReview;
        //Hardcore.isBoughtHardcore = Instance.progressData.isBoughtHardcore;
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
