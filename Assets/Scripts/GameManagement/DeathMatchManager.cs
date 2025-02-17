using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class DeathMatchManager : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI _timeTxt;
    [SerializeField] private float _timeToNextRound = 10;
    private KdManager kdManager;
    [HideInInspector, Networked] public PlayerRef Winner { get; set; }
    [Networked] private GameStates _currentState { get; set; }
    [Networked] private float _timePassed { get; set; }
    [Networked] private bool _spawnedPlayers { get; set; } 
    public int KillLimit = 30;
    public float RoundDurationInSeconds = 600f;
    private GameObject _mainCam;
    private ShowResult _showResult;
    private BasicSpawner _spawner;

    private void Awake()
    {
        _mainCam = Camera.main.gameObject;
    }

    public void Reset()
    {
        if(Runner.IsServer) 
        {
            _timePassed = 0;
            _currentState = GameStates.Running;
        }
    }
    public void Update()
    {
        if (kdManager == null)
        {
            kdManager = FindObjectOfType<KdManager>();
            return; 
        }

        if(_spawner == null) 
        {
            _spawner = FindObjectOfType<BasicSpawner>();
        }
        _timeTxt.text = TimeLeft();
        Debug.Log(_currentState); 
        switch (_currentState)
        {
            case GameStates.Running:
                break;
            case GameStates.EndScreen:
                if (_showResult == null)
                {
                    _showResult = FindObjectOfType<ShowResult>();
                }
                kdManager.ShowEndResult();
                _mainCam.transform.parent = null;
                _mainCam.SetActive(false);
                _showResult.ToggleEndScreen(true);
                break;
            case GameStates.InitNewRound:
                _mainCam.SetActive(true);
                Debug.Log("Habe camera active gesetzt"); 
                break;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)] 
    public void Rpc_StartNewRound() 
    {
        kdManager.HideResult();
        InitilizeNewRound();
        FindObjectOfType<ShowResult>().ToggleEndScreen(false);
    }
    public override void FixedUpdateNetwork()
    {
        CheckGameState(); 
        switch(_currentState) 
        {
            case GameStates.Running:
                HandleRoundTime();
                break;
            case GameStates.EndScreen:
                HandleRoundTime();
                break;
            case GameStates.InitNewRound:
                Rpc_StartNewRound(); 
                break; 
        }
    }
    private void HandleRoundTime() 
    {
        if (Runner.IsClient) 
        {
            return; 
        }
        _timePassed += Runner.DeltaTime; 
    }

    private bool KillLimitReached() 
    {
        if(kdManager == null) 
        {
            return false; 
        }
        return kdManager.GetMVPKills() >= KillLimit; 
    }

    private void InitilizeNewRound()
    {
        FindObjectOfType<KdManager>().ResetScores();
        _spawner.RestartGame(); 
        Reset();
    }
    private void CheckGameState() 
    {
        if((TimeLimitReached(RoundDurationInSeconds) | KillLimitReached()) && _currentState == GameStates.Running) 
        {
            _timePassed = 0;
            _spawner.DespawnPlayer(); 
            Rpc_SetGameState(GameStates.EndScreen);
            return; 
        }

        if (TimeLimitReached(_timeToNextRound) && _currentState == GameStates.EndScreen)
        {
            _timePassed = 0;
            Debug.Log("Should init new round now...");
            if (Runner.IsServer)
            {
                Rpc_SetGameState(GameStates.InitNewRound);
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetGameState(GameStates newState)
    {
        _currentState = newState;
    }

    private bool TimeLimitReached(float time) 
    {
        return _timePassed >= time; 
    }
    public void ResetRound()
    {
        if (Runner.IsServer) 
        {
            _timePassed = 0; 
        }
    }
    private string TimeLeft() 
    {
        float timeInSeconds = (RoundDurationInSeconds - _timePassed); 
        int minutes = Mathf.FloorToInt(timeInSeconds/ 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return  $"{minutes}:{seconds:D2}";
    }

    private IEnumerator WaitForNextRound() 
    {
        yield return new WaitForSeconds(5);
        Rpc_StartNewRound();
    } 
}


public enum GameStates 
{
    Running, 
    EndScreen,
    InitNewRound
}