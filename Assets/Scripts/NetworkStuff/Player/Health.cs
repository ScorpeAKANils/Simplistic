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
    private void FixedUpdate()
    {
        if (_playerCount != BasicSpawner.PlayerCount) 
        {
            _playerCount = BasicSpawner.PlayerCount;
            //_kdManager = FindObjectsOfType<KdManager>().ToList(); 
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

    public void DealDamage(PlayerRef target, float damage, PlayerRef attacker)
    {
        if(target == attacker) 
        {
            Debug.LogError("Spieler hat sich vor Verwirrung selbst verletzt"); 
            return; 
        }

        if(_health <= 0) 
        {
            Debug.Log("Spieler ist bereits tot(client)"); 
        }
        float health = _health - damage;
        Debug.Log("Spieler erleidet schaden auf dem Client"); 
        if (health <= 0) 
        {
            Debug.Log("Message an Server: Gegner tot!");
            Rpc_Die(attacker);
        } else 
        {
            Debug.Log("Message an Server: Gegner erleidet Schaden!");
            Rpc_SetHealth(health); 
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Unreliable, TickAligned = true)]
    private void Rpc_SetHealth(float health) 
    {
        Debug.Log("Health wird auf dem Server Geupdatet");
        _health = health;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.U, TickAligned = true)]
    public void Rpc_Die(PlayerRef attacker) 
    {
        _wM.ResetWeaponCollectionStatus();
        Die(_spawner.GetRandomPos(), _player, attacker);
        KdManager.Instance.AddDeath(_player);
        KdManager.Instance.AddKill(attacker);
    }
}