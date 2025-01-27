using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField]
    private LayerMask _bulletLayer;
    private NetworkRunner _runnerRef;
    private bool _wasHit = false;
    private BasicSpawner _spawner; 

    [Networked]
    private PlayerRef _player { get ; set;  }
    private Vector3 _spawnPos; 

    private void Awake()
    {
        _runnerRef = FindObjectOfType<NetworkRunner>();
        _spawnPos = transform.position; 
    }
    public void SetPlayerRef(PlayerRef player) 
    {
        _player = player; 
    } 

    public PlayerRef GetPlayer() 
    {
        return _player; 
    }

    public void Die(BasicSpawner s) 
    {
        transform.position = s.GetRandomPos();
        RPC_ShowHit("Aua " + s.RunnerRef.LocalPlayer); 

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_ShowHit(string msg, RpcInfo info = default)
    {
        var txt = FindObjectOfType<TextMeshProUGUI>();
        txt.text = msg; 
    }

}
