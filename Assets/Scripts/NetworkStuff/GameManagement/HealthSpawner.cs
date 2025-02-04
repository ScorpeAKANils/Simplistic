using Fusion;
using UnityEngine;

public class HealthSpawner : SimulationBehaviour
{
    [SerializeField, Tooltip("Time for the next Health Pack to spawn in Seconds")] private float _spawnTime = 60;
    [SerializeField] private NetworkPrefabRef _healthPack; 
    private float _currentTime = 60;
    private bool _spawned = false; 
    public void Update()
    {
        if(Runner.IsServer == false | _spawned == true) 
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
        _spawned = true;
        _currentTime = 0; 
        Runner.Spawn(_healthPack, this.transform.position, Quaternion.identity); 
    }
}
