using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLoadScene : NetworkBehaviour
{
    public void StartGame(string name)
    {
        if (IsServer)
        {
            FindFirstObjectByType<ConnectionApproval>().isGameStarted.Value = true;
            NetworkManager.SceneManager.LoadScene(name, LoadSceneMode.Single);
        }
    }

    public void QuitGame()
    {
        if (IsOwner || IsClient)
            QuitGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void QuitGameServerRpc(ServerRpcParams rpcParams = default)
    {
        FindFirstObjectByType<ConnectionApproval>().isGameStarted.Value = false;
        NetworkManager.SceneManager.LoadScene("Multiplayer", LoadSceneMode.Single);
    }
}
