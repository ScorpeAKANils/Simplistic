using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq; 
public class KdManager : NetworkBehaviour
{

    [SerializeField] private AudioSource KillSound;
    [SerializeField] private TextMeshProUGUI _kdText;
    [SerializeField] private GameObject ScoreBoardObj; 
    private BasicSpawner _spawner;
    private Dictionary<PlayerRef, PlayerScore> _playerScores = new();
    private ScoreBoard _board;

    private bool _scoreBoadIsShown = false; 
    [Networked] int _kills { get; set; }
    [Networked] int _deaths { get; set; }
    public override void Spawned () 
    {
        _spawner = FindObjectOfType<BasicSpawner>();
    }


    private void Start()
    {
        _board = FindObjectOfType<ScoreBoard>();
        _board.transform.parent.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && _scoreBoadIsShown == false)
        { 
            _scoreBoadIsShown=true;
            ShowScoreBoard(); 
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && _scoreBoadIsShown == true)
        {
            _scoreBoadIsShown = false; 
            HideScoreBoard(); 
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void Rpc_AddKill(PlayerRef player)
    {
        if (!_playerScores.ContainsKey(player))
        {
            _playerScores.Add(player, new PlayerScore());
        }

        PlayerScore newScore = _playerScores[player];
        newScore.kills++;
        _playerScores[player] = newScore;

        if (Runner.LocalPlayer == player)
        {
            KillSound.Play();
        }

        //Rpc_UpdateScoreboard(); // Scoreboard f?r alle Spieler aktualisieren
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void Rpc_AddDeath(PlayerRef player)
    {
        if (!_playerScores.ContainsKey(player))
        {
            _playerScores.Add(player, new PlayerScore());
        }

        PlayerScore newScore = _playerScores[player];
        newScore.deahts++;
        _playerScores[player] = newScore;

        //Rpc_UpdateScoreboard(); // Scoreboard f?r alle Spieler aktualisieren
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void Rpc_AddPlayerToScoreBoard(PlayerRef newPlayer)
    {
        if (!_playerScores.ContainsKey(newPlayer))
        {
            _playerScores.Add(newPlayer, new PlayerScore()); 
        }

        //Rpc_UpdateScoreboard(); // Neues Scoreboard f?r alle senden
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void Rpc_UpdateScoreboard()
    {
        UpdateScoreboardUI();
    }

    private void UpdateScoreboardUI()
    {
        ShowScoreBoard();
    } 

    public void ShowScoreBoard()
    {
        List<PlayerInfo> sortedPlayer = new();

        foreach (KeyValuePair<PlayerRef, PlayerScore> player in _playerScores)
        {
            sortedPlayer.Add(new PlayerInfo(player.Key, player.Value));
        }

        sortedPlayer.Sort((a, b) => b.score.kills.CompareTo(a.score.kills));
        foreach (var player in sortedPlayer)
        {
            Debug.Log($"Player {player.id} - Kills: {player.score.kills}");
        }
        _board.transform.parent.gameObject.SetActive(true);
        int count = 0;
        foreach (var item in sortedPlayer)
        {
            _board.ScoreTextes[count].text = item.id.ToString() + " - " + item.score.kills.ToString() + " - " + item.score.deahts.ToString();
            count++; 
        }
    }

    public void HideScoreBoard() 
    {
        _board.transform.parent.gameObject.SetActive(false); 
    }
}

public struct PlayerScore 
{
    public int kills;
    public int deahts;
}

public struct PlayerInfo 
{
    public PlayerRef id;
    public PlayerScore score; 

    public PlayerInfo(PlayerRef p, PlayerScore s) 
    {
        id = p;
        score = s; 
    }
}
