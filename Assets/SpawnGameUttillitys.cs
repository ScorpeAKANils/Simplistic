using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGameUttillitys : SimulationBehaviour
{
    [SerializeField] private NetworkPrefabRef _roundManager;
    public bool _spawnedStuff; 
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Runner == null)
            return; 
        if (Runner.IsServer && _spawnedStuff ==false)
        {
            _spawnedStuff = true; 
            Runner.Spawn(_roundManager);
        }
    }
}
