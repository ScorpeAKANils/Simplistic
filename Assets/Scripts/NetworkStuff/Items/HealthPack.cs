using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Item
{
    public override void OnInteract(PlayerRef player)
    {
        Rpc_HealPlayer(player);
        if(DestroyWhenUsed) 
        {
            Runner.Despawn(this.GetComponent<NetworkObject>());
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_HealPlayer(PlayerRef playerToHeal) 
    {
        var spawner = FindObjectOfType<BasicSpawner>();
        spawner.HealPlayer(playerToHeal);
    }
}
