using UnityEngine;

public class InitializeData : MonoBehaviour
{
    private void Awake()
    {
        Progress.Instance.Load();
        Options.Instance.Load();
    }
}
