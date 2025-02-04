using Fusion;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField, Tooltip("Time for the next Item needs to spawn in Seconds")] private float _spawnTime = 60;
    [SerializeField] private NetworkPrefabRef _itemPref;
    private NetworkRunner m_runner; 
    private float _currentTime = 60;
    [SerializeField] private Vector3 SpawnWeaponAt; 
    [Networked] public bool _spawned { get; set; }
    [Networked] public Item Item { get; set; } 
    public void Start() 
    {
        m_runner = FindObjectOfType<NetworkRunner>(); 
    }

    public override void Spawned()
    {
        base.Spawned();
        SpawnWeaponAt = transform.position; 
    }
    public override void FixedUpdateNetwork()
    {
        if(m_runner.IsServer == false | _spawned == true) 
        {
            return; 
        }
        _currentTime += Runner.DeltaTime; 
        if(_currentTime >= _spawnTime) 
        {
            SpawnItem(); 
        }

    }

    private void SpawnItem() 
    {
        if(m_runner.IsServer) 
        {
            _spawned = true;
            _currentTime = 0; 
            var healthPack = m_runner.Spawn(_itemPref, SpawnWeaponAt, Quaternion.identity);
            Item = healthPack.GetComponent<Item>(); 
        }
    }
}
