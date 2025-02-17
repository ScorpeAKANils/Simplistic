using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
public class KdManager : NetworkBehaviour
{

    public static KdManager Instance; 
    [SerializeField] private AudioSource KillSound;
    [SerializeField] private TextMeshProUGUI _kdText;
    [SerializeField] private ScoreBoard _board; 
    [SerializeField] private GameObject _boardCanvas; 
    
    private BasicSpawner _spawner;
    [Networked, Capacity(7)]private NetworkDictionary<PlayerRef, PlayerScore> _playerScores => default;

    private List<PlayerInfo> _sortedPlayer = new();
    private bool _scoreBoadIsShown = false;
    [Networked] public bool IsSpawned { get; set; } = false;
    [Networked] public PlayerRef Mvp { get; set; }
    private bool _pressedTab; 
    public override void Spawned()
    {
        IsSpawned = true; 
    }

    private void Awake()
    {
        if(Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(this); 
        } else 
        {
            Destroy(this.gameObject); 
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && _scoreBoadIsShown == false)
        {
            ShowEndResult(); 
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && _scoreBoadIsShown == true)
        {
            _scoreBoadIsShown = false;
            HideScoreBoard();
        }
    }


    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(Runner.IsServer) 
        {
            Mvp = _sortedPlayer[0].id; 
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

    public void ShowEndResult() 
    {
        UpdateScoreboard();
        _scoreBoadIsShown = true;
        _boardCanvas.SetActive(true);
        ShowScoreBoard();
    }

    public void HideResult()
    {
        _scoreBoadIsShown = false; 
        HideScoreBoard(); 
    }
    public int GetMVPKills() 
    {
        if (_sortedPlayer.Count <= 0)
        {
            return 0;  
        }
        return _sortedPlayer[0].score.kills; 
    }

    public PlayerRef GetMVP()
    {
        if(_sortedPlayer.Count <= 0) 
        {
            return new PlayerRef(); 
        }
        return Mvp;
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

    public void ResetScores()
    {
        if(Runner.IsServer) 
        {
            _sortedPlayer.Clear();
            _playerScores.Clear(); 
        }
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
            ShowScoreBoard(clearScoreBoard, true); 
        }
    }

    public void ShowScoreBoard(bool clearScoreBoard = false, bool allReadyActive = false)
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
        foreach (var item in _sortedPlayer)
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
        _boardCanvas.SetActive(false); 
    }
}