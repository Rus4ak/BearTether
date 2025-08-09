using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PullRope : NetworkBehaviour
{
    [NonSerialized] public NetworkVariable<NetworkObjectReference> PulledPlayer = new NetworkVariable<NetworkObjectReference>();
    [NonSerialized] public NetworkVariable<NetworkObjectReference> PullTo = new NetworkVariable<NetworkObjectReference>();

    [ServerRpc(RequireOwnership = false)]
    public void SetPulledPlayerServerRpc(NetworkObjectReference pulledPlayer)
    {
        PulledPlayer.Value = pulledPlayer;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPullToServerRpc(NetworkObjectReference pullTo)
    {
        PullTo.Value = pullTo;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivatePullServerRpc()
    {
        PulledPlayer.Value = default;
        PullTo.Value = default;
    }
}
