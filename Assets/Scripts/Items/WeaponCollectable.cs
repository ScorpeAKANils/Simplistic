using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; 
public class WeaponCollectable : Item
{
    [SerializeField] private WeaponType _type; 
    public override void OnInteract(PlayerRef player, ItemSpawner i)
    {
        Rpc_CollectedWeapon(player, i);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_CollectedWeapon(PlayerRef p, ItemSpawner i)
    {
        var Spawner = FindObjectOfType<BasicSpawner>();
        Spawner.CollectGun(p, this.GetComponent<NetworkObject>(), i, _type);
    }
}
