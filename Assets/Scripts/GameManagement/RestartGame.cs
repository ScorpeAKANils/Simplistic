using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; 
public class RestartGame : NetworkBehaviour 
{
    [SerializeField] private float _timeToNextRound = 10f;
                     private float _timePassed = 0f;
    [Networked]public bool RoundFinished {get; set;} 


    public void LeaveGame() 
    {
        Runner.Disconnect(Runner.LocalPlayer); 
    }
}
