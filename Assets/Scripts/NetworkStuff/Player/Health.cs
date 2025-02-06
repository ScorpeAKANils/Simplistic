using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using UnityEngine.UI;
public class Health : NetworkBehaviour
{
    [Networked] private PlayerRef _player { get; set; }
    [Networked] private float _health { get; set; } 
    private float _maxHealth = 50;
    [SerializeField] private LayerMask _bulletLayer;
    [SerializeField] private Scrollbar _healthBar;
    [SerializeField] private WeaponManager _wM; 
    private NetworkRunner _runnerRef;
    private bool _wasHit = false;
    private BasicSpawner _spawner;
    private CharacterController _cc;
    private float _previusHealth;
    private Vector3 _spawnPos;
    private bool _died = false; 
    [Networked] private PlayerRef _playerLastHitBy { get; set; }

    public override void Spawned()
    {
        _runnerRef = FindObjectOfType<NetworkRunner>();
        _spawnPos = transform.position;
        _cc = this.GetComponent<CharacterController>();
        _spawner = FindObjectOfType<BasicSpawner>(); 
        _previusHealth = _health;
    }


    public void InitHealth()
    {
        _health = _maxHealth;
    }
    private void Start()
    {
        _healthBar = FindObjectOfType<PlayerHudTag>().GetComponentInChildren<Scrollbar>();

    }
    //[Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, Channel = RpcChannel.Reliable)]
    public void Rpc_UpdateHealthBar(float health)
    {
        var bar = FindObjectOfType<Scrollbar>();
        bar.value = health / _maxHealth;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority)
            return;
        Rpc_UpdateHealthBar(_health); 
        if(_health <= 0) 
        {
            _wM.Weapons[0].ResetCanFire();
            Rpc_Respawn(GetPlayer(), _playerLastHitBy);
        }

        if(_health > 0)
        {
            _died = false; 
        }
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
    //[Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, Channel=RpcChannel.Reliable)]
    public void Die(Vector3 respawnpos, PlayerRef playerDamaged, PlayerRef killer) 
    {
        StaticRpcHolder.Rpc_ShowKillFeed(Runner, killer, playerDamaged);
        this.GetComponent<SimpleKCC>().SetPosition(respawnpos);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, Channel = RpcChannel.Reliable)]
    public void Rpc_HealUp()
    {
        _health = _maxHealth;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    public void RPC_ApplyDamage(PlayerRef target, float damage, PlayerRef attacker)
    {
        //if (Runner.IsServer == false | target == attacker)
        //{
        //    return;
        //}
        if (_health <= 0)
            return; 
        _playerLastHitBy = attacker; 
        _health -= damage; 
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_Respawn(PlayerRef target, PlayerRef attacker) 
    {
        if(Runner.IsServer == false) 
        {
            return; 
        }
        this.GetComponent<WeaponManager>().ResetWeaponCollectionStatus();
        Die(_spawner.GetRandomPos(), target, attacker);
        InitHealth();
        Rpc_UpdateHealthBar(GetHealth());
        foreach (var kd in FindObjectsOfType<KdManager>())
        {
            kd.Rpc_AddDeath(target);
            kd.Rpc_AddKill(attacker);
        }
    }

}
