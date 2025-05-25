using UnityEngine;

public class RoomData
{
    private static RoomData _instantiate;

    public static RoomData Instantiate
    {
        get
        {
            _instantiate ??= new RoomData();

            return _instantiate;
        }
    }

    public string roomName = "Room";
    public string roomCode = "Code";
}
