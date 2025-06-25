using UnityEngine;

public class InitializeData : MonoBehaviour
{
    private void Awake()
    {
        Progress.Instance.Load();
        Options.Instance.Load();

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }
}
