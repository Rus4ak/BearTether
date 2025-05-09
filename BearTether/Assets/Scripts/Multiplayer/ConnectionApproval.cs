using System;
using Unity.Netcode;
using UnityEngine;

public class ConnectionApproval : NetworkBehaviour
{
    [NonSerialized] public NetworkVariable<bool> isGameStarted = new NetworkVariable<bool>(false);

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += OnClientConnecting;
        DontDestroyOnLoad(this);
    }

    private void OnClientConnecting(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (isGameStarted.Value)
        {
            response.Approved = false;
            response.Reason = "Game started";
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = true;
    }
}
