using System;
using System.Collections.Generic;

[Serializable]
public class MapData
{
    public int mapId;
    public bool isUnlock;

    public MapData(int mapId, bool isUnlock)
    {
        this.mapId = mapId;
        this.isUnlock = isUnlock;
    }
}

public class MapsData 
{
    public static List<MapData> maps = new List<MapData>();
    public static List<MapData> mapsMultiplayer = new List<MapData>();
}
