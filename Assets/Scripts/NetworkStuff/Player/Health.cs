using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; 
public class Health : NetworkBehaviour
{
    [Networked] private PlayerRef _player { get; set; }
    [Networked] private float _health { get; set; } 
    [Networked] private PlayerRef _playerLastHitBy { get; set; }
    
    [SerializeField] private LayerMask _bulletLayer;
    [SerializeField] private Scrollbar _healthBar;
    [SerializeField] private WeaponManager _wM; 
    [SerializeField]private SimpleKCC _cc;
    
    private float _maxHealth = 50;
    private BasicSpawner _spawner;
    private int _playerCount = 0;
    private List<KdManager> _kdManager = new(); 
    public override void Spawned()
    {
        _spawner = FindObjectOfType<BasicSpawner>();
        _healthBar = FindObjectOfType<PlayerHudTag>().GetComponentInChildren<Scrollbar>();
    }

    public void InitHealth()
    {
        _health = _maxHealth;
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(HasInputAuthority == false) 
        {
            return; 
        }
        UpdateHealthBar();
    }
    private void FixedUpdate()
    {
        if (_playerCount != BasicSpawner.PlayerCount) 
        {
            _playerCount = BasicSpawner.PlayerCount;
            _kdManager = FindObjectsOfType<KdManager>().ToList(); 
        }
    }
    public void UpdateHealthBar()
    {
        _healthBar.value = _health / _maxHealth;
    }

    public void SetPlayerRef(PlayerRef player)
    {
        _player = player;
    }

    public PlayerRef GetPlayer()
    {
        return _player;
    }

    public float GetHealth()
    {
        return _health;
    }

    public void Die(Vector3 respawnPos, PlayerRef playerDamaged, PlayerRef killer) 
    {
        _cc.SetPosition(respawnPos);
        InitHealth();
        StaticRpcHolder.Rpc_ShowKillFeed(Runner, killer, playerDamaged);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, Channel = RpcChannel.Reliable)]
    public void Rpc_HealUp()
    {
        _health = _maxHealth;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    public void RPC_ApplyDamage(PlayerRef target, float damage, PlayerRef attacker)
    {
        if (Runner.IsServer == false | target == attacker)
        {
            return;
        }
 
        if (_health > 0) 
        {
             _playerLastHitBy = attacker;
             _health -= damage;
        }
        if (_health <= 0)
        {
            _wM.ResetWeaponCollectionStatus();
            Die(_spawner.GetRandomPos(), target, attacker);
           //foreach (KdManager kd in _kdManager)
           //{
           //    if(kd.HasStateAuthority) 
           //    {
           //        kd.Rpc_AddDeath(target);
           //        kd.Rpc_AddKill(attacker);
           //    }
           //}
        }
    }
}