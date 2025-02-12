using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathMatchManager : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI _timeTxt;
    public float RoundDurationInSeconds = 600f;
    [HideInInspector, Networked] public PlayerRef Winner { get; set; }
    public int KillLimit = 30;

    [SerializeField] private KdManager kdManager;

    [Networked] private float _timePassed { get; set; }
    [Networked] private bool _spawned { get; set; }
    public static DeathMatchManager Instance; 
    private void Awake()
    {
        if(Instance == null) 
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(this.gameObject); 
        }
    }
    public void Update()
    {
        if (kdManager == null)
        {
            kdManager = FindObjectOfType<KdManager>();
            return; 
        }
        _timeTxt.text = TimeLeft();
    }
    public override void FixedUpdateNetwork()
    {
        if(Runner.IsServer == false) 
        {
            return; 
        }
        _timePassed += Runner.DeltaTime; 
        if(_timePassed >= RoundDurationInSeconds | kdManager.GetMVPKills()>=KillLimit) 
        {
            Runner.LoadScene("EndScreen", loadSceneMode: UnityEngine.SceneManagement.LoadSceneMode.Single, setActiveOnLoad: true); 
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
