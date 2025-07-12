using Unity.Netcode;
using UnityEngine;

public class NetworkQuit : MonoBehaviour
{
    void Start()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.Shutdown();

        foreach (var obj in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (obj.name != "LoadingScreen")
                Destroy(obj.gameObject);
        }

        Destroy(GameObject.Find("NetworkManager"));
        Destroy(GameObject.Find("SessionManager"));
        Destroy(GameObject.Find("WidgetEventDispatcher"));
    }
}
