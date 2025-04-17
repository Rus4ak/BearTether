using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLoadScene : NetworkBehaviour
{
    public void Load(string name)
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene(name, LoadSceneMode.Single);
        }
    }
}
