using UnityEngine;

public class SoundVolume
{
    private static SoundVolume _instance;

    public static SoundVolume Instance
    {
        get
        {
            _instance ??= new SoundVolume();
            
            return _instance;
        }
    }

    public float volume = .7f;
}
