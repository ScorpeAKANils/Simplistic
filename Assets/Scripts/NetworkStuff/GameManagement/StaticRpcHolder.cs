using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StaticRpcHolder : NetworkBehaviour
{  
    [Rpc(RpcSources.InputAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public static void RPC_SendHitInfo(NetworkRunner runner, PlayerRef enemy, PlayerRef self, FireBullet gunRef, RpcInfo info = default)
    {
        gunRef.Spawner.ErasePlayer(enemy, self);
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public static void RPC_VisualieShot(NetworkRunner runner, FireBullet gunRef, PlayerRef player, RpcInfo info = default)
    {
        gunRef.LineRenderer.enabled = true;
        Vector3 endPos = gunRef.GunBarrel.position + (gunRef.GunBarrel.forward * 100f);
        gunRef.LineRenderer.SetPosition(0, gunRef.GunBarrel.position);
        gunRef.LineRenderer.SetPosition(1, endPos);
        gunRef.AmmoInMag--;
        Debug.Log(player + " Ammo left: " + gunRef.AmmoInMag);

    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public static void RPC_DisableDebugLine(NetworkRunner runner, FireBullet gunRef, RpcInfo info = default)
    {
        gunRef.LineRenderer.enabled = false;
    }
    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public static void Rpc_ShowKillFeed(NetworkRunner runner, PlayerRef enemy, PlayerRef self) 
    {
        KillFeedManager.ShowKillMessage(self, enemy); 
    }
    public static IEnumerator TextLifeTime(TextMeshProUGUI text, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(text);
    }
}
