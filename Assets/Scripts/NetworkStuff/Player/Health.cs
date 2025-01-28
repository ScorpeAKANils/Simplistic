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
    private CharacterController _cc; 
    [Networked]
    private PlayerRef _player { get ; set;  }
    private Vector3 _spawnPos; 

    private void Awake()
    {
        _runnerRef = FindObjectOfType<NetworkRunner>();
        _spawnPos = transform.position; 
        _cc = this.GetComponent<CharacterController>();
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
        Vector3 pos = s.GetRandomPos();
        Debug.Log(pos);
        _cc.enabled = false; 
        transform.position = pos;
        _cc.enabled = true;

    }

}
