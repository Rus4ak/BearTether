using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLoadScene : NetworkBehaviour
{
    public void Load(string name)
    {
        if (IsServer)
        {
            FindFirstObjectByType<ConnectionApproval>().isGameStarted.Value = true;
            NetworkManager.SceneManager.LoadScene(name, LoadSceneMode.Single);
        }
    }
}
