using Fusion;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField, Tooltip("Time for the next Item needs to spawn in Seconds")] private float _spawnTime = 60;
    [SerializeField] private NetworkPrefabRef _itemPref;
    private NetworkRunner m_runner; 
    private float _currentTime = 60;
    [Networked] private bool _spawned { get; set; }
    [Networked] public Item Item { get; set; } 
    public void Start() 
    {
        m_runner = FindObjectOfType<NetworkRunner>(); 
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
            SpawnHealthPack(); 
        }

    }

    private void SpawnHealthPack() 
    {
        if(m_runner.IsServer) 
        {
            _spawned = true;
            _currentTime = 0; 
            var healthPack = m_runner.Spawn(_itemPref, this.transform.position, Quaternion.identity);
            Item = healthPack.GetComponent<HealthPack>(); 
        }
    }
}
