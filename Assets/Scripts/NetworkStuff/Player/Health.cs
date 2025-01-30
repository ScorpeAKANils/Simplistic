using Fusion;
using UnityEngine;
using UnityEngine.UI;
public class Health : NetworkBehaviour
{
    [Networked] private PlayerRef _player { get ; set;  }
    private float _health { get; set; } = 50;
    private float _maxHealth = 50;
    [SerializeField] private LayerMask _bulletLayer;
    [SerializeField] private Scrollbar _healthBar;
    private NetworkRunner _runnerRef;
    private bool _wasHit = false;
    private BasicSpawner _spawner;
    private CharacterController _cc; 
    private Vector3 _spawnPos; 

    private void Awake()
    {
        _runnerRef = FindObjectOfType<NetworkRunner>();
        _spawnPos = transform.position; 
        _cc = this.GetComponent<CharacterController>();
    }

    private void Start()
    {
        _healthBar = FindObjectOfType<PlayerHudTag>().GetComponentInChildren<Scrollbar>(); 
    }
    public void SetPlayerRef(PlayerRef player) 
    {
        _player = player; 
    }
    public PlayerRef GetPlayer() 
    {
        return _player; 
    }
    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void Rpc_GetDamage(Vector3 respawnpos,  float damage, PlayerRef playerDamaged, PlayerRef killer) 
    {
            _health -= damage;
            _healthBar.value = _health / _maxHealth;
            if (_health <= 0)
            {
                _cc.enabled = false;
                transform.position = respawnpos;
                _cc.enabled = true;
                _health = _maxHealth;
                _healthBar.value = _health / _maxHealth;
                StaticRpcHolder.Rpc_ShowKillFeed(Runner, killer, playerDamaged); 
            }
    }

}
