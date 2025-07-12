using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : NetworkBehaviour
{
    [NonSerialized] public NetworkVariable<int> countInitialized = new NetworkVariable<int>();
    [NonSerialized] public bool isActive = false;

    public static LoadingScreen Instance;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Multiplayer")
            if (!IsOwner)
                return;

        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void SetOn()
    {
        AudioListener.volume = 0f;
        transform.GetChild(0).gameObject.SetActive(true);
        isActive = true;
    }

    public void SetOff()
    {
        Invoke(nameof(SetOffScreen), 1f);
    }

    private void SetOffScreen()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        isActive = false;
        AudioListener.volume = 1f;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddInitializeServerRpc()
    {
        countInitialized.Value++;
    }

    [ServerRpc]
    public void RemoveInitializeServerRpc()
    {
        SetOffClientRpc();
        countInitialized.Value = 0;
    }

    [ClientRpc]
    public void SetOffClientRpc() 
    {
        SetOff();
    }
}
