using System;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject _levelsMenu;
    [SerializeField] private GameObject _unclockMenu;
    [SerializeField] private int _mapId;
    [SerializeField] private GameMode _gamemode;

    [NonSerialized] public bool isUnlock = false;

    private void Awake()
    {
        int countSimilar = 0;

        if (_gamemode == GameMode.Singleplayer || _gamemode == GameMode.SingleplayerHardcore)
        {
            foreach (MapData mapData in MapsData.maps) 
                if (mapData.mapId == _mapId)
                {
                    countSimilar++;
                    isUnlock = mapData.isUnlock;
                }
            
            if (countSimilar == 0)
            {
                MapsData.maps.Add(new MapData(_mapId, isUnlock));
            }
        }
        else 
        {
            foreach (MapData mapData in MapsData.mapsMultiplayer)
                if (mapData.mapId == _mapId)
                {
                    countSimilar++;
                    isUnlock = mapData.isUnlock;
                }

            if (countSimilar == 0)
            {
                MapsData.mapsMultiplayer.Add(new MapData(_mapId, isUnlock));
            }
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (isUnlock)
        {
            _unclockMenu.SetActive(false);
            _levelsMenu.SetActive(true);
        }
        else
        {
            _unclockMenu.SetActive(true);
            _levelsMenu.SetActive(false);
        }
    }

    public void UnlockMap()
    {
        if (Bank.Instance.Coins >= 100)
        {
            Bank.Instance.Coins -= 100;
            Progress.Instance.progressData.bank = Bank.Instance.Coins;

            isUnlock = true;

            Initialize();

            if (_gamemode == GameMode.Singleplayer || _gamemode == GameMode.SingleplayerHardcore)
            {
                foreach (MapData mapData in MapsData.maps)
                    if (mapData.mapId == _mapId)
                        mapData.isUnlock = true;

                Progress.Instance.progressData.maps = MapsData.maps;
            }
            else 
            {
                foreach (MapData mapData in MapsData.mapsMultiplayer)
                    if (mapData.mapId == _mapId)
                        mapData.isUnlock = true;

                Progress.Instance.progressData.mapsMultiplayer = MapsData.mapsMultiplayer;
            }

            Progress.Instance.Save();
        }
    }
}
