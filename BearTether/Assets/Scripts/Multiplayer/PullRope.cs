using System;
using Unity.Netcode;

public class PullRope : NetworkBehaviour
{
    [NonSerialized] public NetworkVariable<NetworkObjectReference> PulledPlayer = new NetworkVariable<NetworkObjectReference>();
    [NonSerialized] public NetworkVariable<NetworkObjectReference> PullTo = new NetworkVariable<NetworkObjectReference>();

    [ServerRpc(RequireOwnership = false)]
    public void ActivatePullServerRpc(NetworkObjectReference pulledPlayer, NetworkObjectReference pullTo)
    {
        if (!PulledPlayer.Value.TryGet(out _))
        {
            PulledPlayer.Value = pulledPlayer;
            PullTo.Value = pullTo;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivatePullServerRpc()
    {
        PulledPlayer.Value = default;
        PullTo.Value = default;
    }
}
