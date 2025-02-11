using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMatchManager : SimulationBehaviour
{

    public float RoundDurationInSeconds = 600f;
    public int KillLimit = 30;

    [SerializeField] private KdManager kdManager;

    [Networked] private float _timePassed { get; set; } 
    
    void Update()
    {
        if(Runner.IsServer == false) 
        {
            return; 
        }
        _timePassed += Runner.DeltaTime; 
        if(_timePassed >= RoundDurationInSeconds | kdManager.GetMVPKills()>=KillLimit) 
        {
        
        }
    }
}
