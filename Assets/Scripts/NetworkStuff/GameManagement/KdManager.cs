using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq; 
public class KdManager : NetworkBehaviour
{

    public static KdManager Instance; 
    [SerializeField] private AudioSource KillSound;
    [SerializeField] private TextMeshProUGUI _kdText;
    
    private BasicSpawner _spawner;
    [Networked, Capacity(7)]private NetworkDictionary<PlayerRef, PlayerScore> _playerScores => default;
    [SerializeField] private ScoreBoard _board;
    private List<PlayerInfo> _sortedPlayer = new();
    private bool _scoreBoadIsShown = false;

    private void Start()
    {
    }

    private void Awake()
    {
        if(Instance == null) 
        {
            Instance = this; 
        } else 
        {
            Destroy(this.gameObject); 
        }
        _board.transform.parent.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && _scoreBoadIsShown == false)
        { 
            _scoreBoadIsShown=true;
            _board.transform.parent.gameObject.SetActive(true);
            UpdateScoreboard();
            ShowScoreBoard(_sortedPlayer); 
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && _scoreBoadIsShown == true)
        {
            _scoreBoadIsShown = false; 
            HideScoreBoard(); 
        }
    }
    public void AddKill(PlayerRef player)
    {
        if (!_playerScores.ContainsKey(player))
        {
            _playerScores.Add(player, new PlayerScore());
        }

        PlayerScore newScore = _playerScores[player];
        newScore.kills++;
        _playerScores.Set(player, newScore);
        UpdateScoreboard();
    }

    public void AddDeath(PlayerRef player)
    {
        if (!_playerScores.ContainsKey(player))
        {
            _playerScores.Add(player, new PlayerScore());
        }

        PlayerScore newScore = _playerScores[player];
        newScore.deahts++;
        _playerScores.Set(player, newScore);

        UpdateScoreboard();
    }

    public void AddPlayerToScoreBoard(PlayerRef newPlayer)
    {
        if (!_playerScores.ContainsKey(newPlayer))
        {
            _playerScores.Add(newPlayer, new PlayerScore()); 
        }
        UpdateScoreboard();
    }

    public void RemovePlayerFromScoreBoard(PlayerRef playerToRemove)
    {
        if (!_playerScores.ContainsKey(playerToRemove))
        {
            _playerScores.Remove(playerToRemove); 
        }
        UpdateScoreboard(true);
    }

    private void UpdateScoreboard(bool clearScoreBoard = false)
    {
        _sortedPlayer.Clear();
        foreach (KeyValuePair<PlayerRef, PlayerScore> player in _playerScores)
        {
            _sortedPlayer.Add(new PlayerInfo(player.Key, player.Value));
        }

        _sortedPlayer.Sort((a, b) => b.score.kills.CompareTo(a.score.kills));

        if (_scoreBoadIsShown)
        {
            ShowScoreBoard(_sortedPlayer, clearScoreBoard, true); 
        }
    }

    public void ShowScoreBoard(List<PlayerInfo> players, bool clearScoreBoard = false, bool allReadyActive = false)
    {
        if (allReadyActive == false)
        {
            _board.transform.parent.gameObject.SetActive(true); 
        }
        if (clearScoreBoard)
        {
            ClearScoreBoard();
        }
        int count = 0;
        foreach (var item in players)
        {
            if (Runner.LocalPlayer == item.id)
            {
                _board.ScoreTextes[count].color = Color.yellow;
            }
            else
            {
                _board.ScoreTextes[count].color = Color.white;
            }
            _board.ScoreTextes[count].text = item.id.ToString() + " - " + item.score.kills.ToString() + " - " + item.score.deahts.ToString();
            count++;
        }
    }

    public void ClearScoreBoard()
    {
        foreach (TextMeshProUGUI text in _board.ScoreTextes)
        {
            if (text.text != string.Empty)
            {
                text.text = string.Empty;
                continue;
            }
            break;
        }
    }
    public void HideScoreBoard() 
    {
        _board.transform.parent.gameObject.SetActive(false); 
    }
}