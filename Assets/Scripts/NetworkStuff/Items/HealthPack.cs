using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Item
{
    public override void OnInteract(PlayerRef player, ItemSpawner i)
    {
        Rpc_HealPlayer(player, i);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_HealPlayer(PlayerRef playerToHeal, ItemSpawner i) 
    {
        var spawner = FindObjectOfType<BasicSpawner>();
        spawner.HealPlayer(playerToHeal, this.GetComponent<NetworkObject>(), i);
    }
}
