using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private List<Transform> transforms = new();
    [SerializeField] private Camera _cam; 
    [Networked] int _spawnIndex { get; set; }
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();
    private Dictionary<PlayerRef, Health> _playersHealths = new(); 
    private NetworkRunner _runnerRef;
    public  NetworkRunner RunnerRef;
    private Vector2 _inputMovement; 
    private float _mouseY;
    private float _mouseX;
    private bool _jump; 
    void Update() 
    {
        _mouseY = Input.GetAxis("Mouse X");
        _mouseX = Input.GetAxis("Mouse Y");
        _inputMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); 

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            _jump = true; 
        }
    }
   
    public void ErasePlayer(PlayerRef player, PlayerRef killer) 
    {
        if (_runnerRef.IsServer) 
        {
            _playersHealths[player].Rpc_GetDamage(GetRandomPos(), _playersHealths[player], 10f, player, killer); 
        }
    }
    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runnerRef = gameObject.AddComponent<NetworkRunner>();
        _runnerRef.ProvideInput = true;
        RunnerRef = _runnerRef;
        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runnerRef.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnGUI()
    {
        if (_runnerRef == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        { 
            Vector3 pos = GetRandomPos();
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, pos, Quaternion.identity, player);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
            Health playerHealth = networkPlayerObject.GetComponent<Health>();
            _playersHealths.Add(player, playerHealth);
            playerHealth.SetPlayerRef(player);
            Debug.Log(playerHealth.GetPlayer());
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Mathf.Abs(_inputMovement.y) > 0.02f)
        {
            data.direction += Vector3.forward * _inputMovement.y;
        } 

        if (Mathf.Abs(_inputMovement.x) > 0.02f)
        {
            data.direction += Vector3.right * _inputMovement.x;
        }

        if (_jump) 
        {
            _jump = false; 
            data.Jump = true; 
        }
        data.MouseY = _mouseY;
        data.MouseX = _mouseX; 
        input.Set(data);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public Vector3 GetRandomPos()
    {
        if (_runnerRef.IsServer) 
        {
            _spawnIndex = UnityEngine.Random.Range(0, transforms.Count - 1);
        }
        return transforms[_spawnIndex].position;
    }

}