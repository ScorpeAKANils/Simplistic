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
    
    [SerializeField] private Scrollbar _healthBar;
    [SerializeField] private WeaponManager _wM; 
    [SerializeField] private SimpleKCC _cc;
    [SerializeField] private KdManager _kdManager; 
    
    private float _maxHealth = 50;
    private BasicSpawner _spawner;
    private int _playerCount = 0;
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

    public void DealDamage(PlayerRef target, float damage, PlayerRef attacker)
    {
        if(attacker == target) 
        {
            Debug.LogError("Play hit him self"); 
            return; 
        }
        if((_health- damage)<= 0) 
        {
            HandlePlayerDeath(attacker);
            return; 
        }
        _health -= damage; 
        if(_health <= 0)
        {
            HandlePlayerDeath(attacker);
        }
    }

    public void HandlePlayerDeath(PlayerRef attacker) 
    {
        _wM.ResetWeaponCollectionStatus();
        Die(_spawner.GetRandomPos(), _player, attacker);
        KdManager.Instance.AddDeath(_player);
        KdManager.Instance.AddKill(attacker);
    }
}