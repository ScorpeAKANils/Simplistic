using Fusion;
using UnityEngine;

public class HealthSpawner : NetworkBehaviour
{
    [SerializeField, Tooltip("Time for the next Health Pack to spawn in Seconds")] private float _spawnTime = 60;
    [SerializeField] private NetworkPrefabRef _healthPack;
    private NetworkRunner m_runner; 
    private float _currentTime = 60;
    private bool _spawned = false; 
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
            m_runner.Spawn(_healthPack, this.transform.position, Quaternion.identity); 
        }
    }
}
