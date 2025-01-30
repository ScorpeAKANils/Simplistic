using Fusion;
using UnityEngine;
using UnityEngine.UI;
public class Health : NetworkBehaviour
{
    [Networked] private PlayerRef _player { get ; set;  }
    [Networked] private float _health { get; set; } = 50;
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
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    private void Rpc_UpdateHealthBar(PlayerRef p)
    {
        if(Runner.LocalPlayer == p)
            _healthBar.value = _health; 
    }
    public void SetPlayerRef(PlayerRef player) 
    {
        _player = player; 
    }
    public PlayerRef GetPlayer() 
    {
        return _player; 
    }
    public void GetDamage(Vector3 respawnpos, Health health, float damage, PlayerRef playerDamaged, PlayerRef killer) 
    {
        this._health -= damage;
        if (_health <= 0) 
        {
            StaticRpcHolder.Rpc_ShowKillFeed(Runner, killer, playerDamaged);
            _cc.enabled = false;
            transform.position = respawnpos;
            _cc.enabled = true;
            Rpc_HealUp(); 
        }
        Rpc_UpdateHealthBar(playerDamaged);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    public void Rpc_HealUp()
    {
        _health = _maxHealth;
    }
}
