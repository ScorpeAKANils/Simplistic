using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathMatchManager : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI _leadingPlayerTxt;
    [SerializeField] TextMeshProUGUI _timeTxt;
    public float RoundDurationInSeconds = 600f;
    [HideInInspector, Networked] public PlayerRef Winner { get; set; }
    public int KillLimit = 30;

    [SerializeField] private KdManager kdManager;

    [Networked] private float _timePassed { get; set; }
    private bool _spawned = false; 
    private void Start()
    {
        DontDestroyOnLoad(gameObject); 
    }

    public override void Spawned()
    {
        base.Spawned();
        _spawned = true; 
    }
    void Update()
    {
        if(_spawned == false) 
        {
            return; 
        }
        _leadingPlayerTxt.text = "Leading Player: " + kdManager.GetMVP().ToString();
        _timeTxt.text = TimeLeft(); 
        if(Runner.IsServer == false) 
        {
            return; 
        }
        _timePassed += Runner.DeltaTime; 
        if(_timePassed >= RoundDurationInSeconds | kdManager.GetMVPKills()>=KillLimit) 
        {
            var server = FindObjectOfType<BasicSpawner>();
            Winner = kdManager.GetMVP();
            server.LoadNewScene("Assets/EndScreen.unity"); 
        }
    }

    private string TimeLeft() 
    {
        float timeInSeconds = (RoundDurationInSeconds - _timePassed); 
        int minutes = Mathf.FloorToInt(timeInSeconds/ 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return  $"{minutes}:{seconds:D2}";
    }
}
