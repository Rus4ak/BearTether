using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkLoadScene : NetworkBehaviour
{
    public void StartGame(string name)
    {
        if (IsOwner || IsClient)
            StartGameServerRpc(name);
    }

    public void QuitGame()
    {
        if (IsOwner || IsClient)
            QuitGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void QuitGameServerRpc(ServerRpcParams rpcParams = default)
    {
        SetLoadingScreenClientRpc();
        FindFirstObjectByType<ConnectionApproval>().isGameStarted.Value = false;
        NetworkManager.SceneManager.LoadScene("Multiplayer", LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc(string name, ServerRpcParams rpcParams = default)
    {
        SetLoadingScreenClientRpc();
        FindFirstObjectByType<ConnectionApproval>().isGameStarted.Value = true;
        NetworkManager.SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    [ClientRpc(RequireOwnership = false)]
    private void SetLoadingScreenClientRpc(ClientRpcParams rpcParams = default)
    {
        LoadingScreen.Instance.SetOn();
    }
}
