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
    private Vector3 _spawnPos; 

    private void Awake()
    {
 
        _healthBar.value = _health/_maxHealth;
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

    public void GetDamage(BasicSpawner s, float damage) 
    {
        _health -= damage;
        _healthBar.value = _health / _maxHealth;
        if (_health <= 0) 
        {
            Vector3 pos = s.GetRandomPos();
            Debug.Log(pos);
            _cc.enabled = false; 
            transform.position = pos;
            _cc.enabled = true;
        }

    }

}
