using Fusion;
using UnityEngine;
using UnityEngine.UI;
public class Health : NetworkBehaviour
{
    [Networked] private PlayerRef _player { get ; set;  }
    [Networked] private float _health { get; set; } = 50f; 
    private float _maxHealth = 50;
    [SerializeField] private LayerMask _bulletLayer;
    [SerializeField] private Scrollbar _healthBar;
    private NetworkRunner _runnerRef;
    private bool _wasHit = false;
    private BasicSpawner _spawner;
    private CharacterController _cc;
    private float _previusHealth; 
    private Vector3 _spawnPos; 

    private void Awake()
    {
        _runnerRef = FindObjectOfType<NetworkRunner>();
        _spawnPos = transform.position; 
        _cc = this.GetComponent<CharacterController>();
        _previusHealth = _health; 
    }

    private void Start()
    {
        _healthBar = FindObjectOfType<PlayerHudTag>().GetComponentInChildren<Scrollbar>(); 

    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, Channel = RpcChannel.Reliable)]
    public void Rpc_UpdateHealthBar(float health)
    {
        var bar = FindObjectOfType<Scrollbar>(); 
        bar.value = health / _maxHealth;
    }

    public void SetPlayerRef(PlayerRef player) 
    {
        _player = player; 
    }

    public PlayerRef GetPlayer() 
    {
        return _player; 
    }
    public float GetDamage(float damage) 
    {
        this._health -= damage;
        return this._health; 
    }

    public float GetHealth() 
    {
        return _health; 
    }

    public void Die(Vector3 respawnpos, PlayerRef playerDamaged, PlayerRef killer) 
    {
        StaticRpcHolder.Rpc_ShowKillFeed(Runner, killer, playerDamaged);
        _cc.enabled = false;
        transform.position = respawnpos;
        _cc.enabled = true;
        Rpc_HealUp();
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    public void Rpc_HealUp()
    {
        _health = _maxHealth;
    }


}
