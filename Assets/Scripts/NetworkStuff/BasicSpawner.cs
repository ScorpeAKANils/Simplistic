using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private List<Transform> transforms = new();
    [SerializeField] private Camera _cam; 
    [Networked] int _spawnIndex { get; set; }

    [SerializeField] private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();
    private Dictionary<PlayerRef, Health> _playersHealths = new(); 
    private NetworkRunner _runnerRef;
    public  NetworkRunner RunnerRef;
    private NetworkInputData _input = new NetworkInputData();

    public void ErasePlayer(PlayerRef player, PlayerRef killer)
    {
        if (_runnerRef.IsServer)
        {
            float playerHealth = _playersHealths[player].GetDamage(10f);
            if (playerHealth <= 0)
            {
                _playersHealths[player].Die(GetRandomPos(), player, killer);
                var kdPlayerDead = _playersHealths[player].GetComponent<KdManager>();
                var kdManagers = FindObjectsOfType<KdManager>();
                foreach (var kd in kdManagers)
                {
                    kd.Rpc_AddDeath(player);
                }
                var kdOther = _playersHealths[killer].GetComponent<KdManager>();
                foreach (var kd in kdManagers)
                {
                    kd.Rpc_AddKill(killer);
                }
                _playersHealths[player].Rpc_UpdateHealthBar(ReturnPlayerHealth(player));
                return;
            }
            _playersHealths[player].Rpc_UpdateHealthBar(ReturnPlayerHealth(player));
        }
    }

    public float ReturnPlayerHealth(PlayerRef player) 
    {
        return _playersHealths[player].GetHealth(); 
    }
    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runnerRef = gameObject.GetComponent<NetworkRunner>();
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

            _spawnedCharacters.Add(player, networkPlayerObject);
            Health playerHealth = networkPlayerObject.GetComponent<Health>();
            _playersHealths.Add(player, playerHealth);
            playerHealth.SetPlayerRef(player);
            playerHealth.InitHealth();

            var kdManagers = FindObjectsOfType<KdManager>(); 
            foreach(var kd in kdManagers) 
            {
                foreach(var existingPlayer in _playersHealths.Values) 
                {
                    kd.Rpc_AddPlayerToScoreBoard(existingPlayer.GetPlayer()); 
                    Debug.Log("Added: " + existingPlayer.GetPlayer()); 
                }
            }
        }

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
        }
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
    }

    public void Update()
    {
        _input.Buttons.Set(MyButtons.Forward, Input.GetKey(KeyCode.W));
        _input.Buttons.Set(MyButtons.Left, Input.GetKey(KeyCode.A));
        _input.Buttons.Set(MyButtons.Backward, Input.GetKey(KeyCode.S));
        _input.Buttons.Set(MyButtons.Right, Input.GetKey(KeyCode.D));
        _input.Buttons.Set(MyButtons.Jump, Input.GetButton("Jump"));
        _input.Buttons.Set(MyButtons.Shooting, Input.GetButton("Fire1"));

        if (Cursor.lockState != CursorLockMode.Locked)
            return;
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            Vector2 lookRotationDelta = new(mouseDelta.y, mouseDelta.x);
            _input.AimDirection += lookRotationDelta; 
        }

    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(_input);
        _input = default;
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
        var players = FindObjectsOfType<PlayerMovement>();
        int randomIndex = UnityEngine.Random.Range(0, transforms.Count - 1);
        for(int i = randomIndex; i < transforms.Count - 1; i++) 
        {
            foreach(var p in players) 
            {
                if ((transforms[i].position-p.transform.position).sqrMagnitude > (30*30f)) 
                {
                    return transforms[i].position;
                }
            }
        }
        return transforms[randomIndex].position; 
    }

}