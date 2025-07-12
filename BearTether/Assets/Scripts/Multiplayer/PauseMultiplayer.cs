using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMultiplayer : NetworkBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    public void SetPause()
    {
        if (IsOwner || IsClient)
        {
            foreach (var player in NetworkPlayersManager.Instance.players)
            {
                player.player.GetComponent<NetworkPlayer>().ActivatePauseMark();
            }

            _pauseMenu.SetActive(true);
            PauseServerRpc(0);
        }
    }

    public void Resume()
    {
        if (IsOwner || IsClient)
        {
            foreach (var player in NetworkPlayersManager.Instance.players)
            {
                player.player.GetComponent<NetworkPlayer>().DisablePauseMark();
            }

            PauseServerRpc(1);
            _pauseMenu.SetActive(false);
        }
    }

    public void MainMenu()
    {
        if (IsOwner || IsClient)
        {
            PauseServerRpc(1);
            QuitGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseServerRpc(int value)
    {
        PauseClientRpc(value);
    }

    [ClientRpc]
    private void PauseClientRpc(int value)
    {
        Time.timeScale = value;
    }

    [ServerRpc(RequireOwnership = false)]
    private void QuitGameServerRpc(ServerRpcParams rpcParams = default)
    {
        SetLoadingScreenClientRpc();
        FindFirstObjectByType<ConnectionApproval>().isGameStarted.Value = false;
        NetworkManager.SceneManager.LoadScene("Multiplayer", LoadSceneMode.Single);
    }

    [ClientRpc]
    private void SetLoadingScreenClientRpc()
    {
        LoadingScreen.Instance.SetOn();
    }
}
