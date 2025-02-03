using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StaticRpcHolder : NetworkBehaviour
{  
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
