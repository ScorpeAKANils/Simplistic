using UnityEngine;
using TMPro; 

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject _startButtons;
    [SerializeField] private GameObject _hostGameButtons;
    [SerializeField] private GameObject _joinGameButtons;
    [SerializeField] private TMP_InputField _roomNameCreateGame;
    [SerializeField] private TMP_InputField _roomNameJoinGame;
    [SerializeField] private BasicSpawner _serverManager; 

    public void HostGamePressed() 
    {
        _startButtons.SetActive(false); 
        _joinGameButtons.SetActive(false);
        _hostGameButtons.SetActive(true); 
    }

    public void JoinGamePressed()
    {
        _startButtons.SetActive(false);
        _hostGameButtons.SetActive(false);
        _joinGameButtons.SetActive(true);
    }
    public void BackPressed()
    {
        _hostGameButtons.SetActive(false);
        _joinGameButtons.SetActive(false);
        _startButtons.SetActive(true);
    }

    public void CreateGame() 
    {
        if(_roomNameCreateGame.text != string.Empty) 
        {
            _serverManager.RoomName = _roomNameCreateGame.text;
            _serverManager.StartGame(Fusion.GameMode.Host);
            return; 
        }
        Debug.LogError("Roomname cant be empty!"); 
    }

    public void JoinGame()
    {
        if (_roomNameJoinGame.text != string.Empty)
        {
            _serverManager.RoomName = _roomNameJoinGame.text;
            _serverManager.StartGame(Fusion.GameMode.Client);
            return;
        }
        Debug.LogError("Roomname cant be empty!");
    }
    public void QuitGamePressed()
    {
        Application.Quit(); 
    }
}
