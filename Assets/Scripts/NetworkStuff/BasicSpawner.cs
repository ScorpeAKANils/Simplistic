using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro; 

public class BasicSpawner : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private List<Transform> transforms = new();
    [SerializeField] private Camera _cam;
    private int _spawnIndex = 0; 

    [SerializeField] private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();
    private Dictionary<PlayerRef, Health> _playersHealths = new(); 
    private NetworkRunner _runnerRef;
    public  NetworkRunner RunnerRef;
    private NetworkInputData _input = new NetworkInputData();
    private TextMeshProUGUI _kdText;
    private bool _resetInput;

    void Start()
    {
        _kdText = FindObjectOfType<KdTagText>().GetComponent<TextMeshProUGUI>();
    }

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void RPC_ApplyDamage(PlayerRef target, float damage, PlayerRef attacker)
    {
        if(Runner.IsServer == false | target == attacker) 
        {
            return; 
        }
        if (_playersHealths.ContainsKey(target))
        {

            float newHealth = _playersHealths[target].GetDamage(damage);
            _playersHealths[target].Rpc_UpdateHealthBar(newHealth);

            if (newHealth <= 0)
            {
                _playersHealths[target].Rpc_Die(GetRandomPos(), target, attacker);
                _playersHealths[target].InitHealth();
                _playersHealths[target].Rpc_UpdateHealthBar(_playersHealths[target].GetHealth());
                foreach (var kd in FindObjectsOfType<KdManager>())
                {
                    kd.Rpc_AddDeath(target);
                    kd.Rpc_AddKill(attacker);
                }
            }
        }
    }
    public float ReturnPlayerHealth(PlayerRef player) 
    {
        return _playersHealths[player].GetHealth(); 
    }
    async void StartGame(GameMode mode)
    {
        _runnerRef = GetComponent<NetworkRunner>(); 
        // Create the Fusion runner and let it know that we will be providing user input
        var networkEvents = this.gameObject.GetComponent<NetworkEvents>();
        networkEvents.OnInput.AddListener(OnInput);
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
        var kdManagers = FindObjectsOfType<KdManager>();
        foreach (var kd in kdManagers)
        {
            kd.Rpc_RemovePlayerFromScoreBoard(player); 
        }
        _playersHealths.Remove(player); 
        _spawnedCharacters.Remove(player);
    }

    void IBeforeUpdate.BeforeUpdate()
    {
        if (_runnerRef != null)
        {
               float pingRaw =  (float)_runnerRef.GetPlayerRtt(_runnerRef.LocalPlayer) * 1000;
            int ping = Mathf.RoundToInt(pingRaw);
            _kdText.text = ping.ToString(); 
        }
        if (_resetInput)
        {
            _resetInput = false; 
            _input = default;
        }
        if (Cursor.lockState != CursorLockMode.Locked)
            return;
        _input.Buttons.Set(MyButtons.Forward, Input.GetKey(KeyCode.W));
        _input.Buttons.Set(MyButtons.Left, Input.GetKey(KeyCode.A));
        _input.Buttons.Set(MyButtons.Backward, Input.GetKey(KeyCode.S));
        _input.Buttons.Set(MyButtons.Right, Input.GetKey(KeyCode.D));
        _input.Buttons.Set(MyButtons.Jump, Input.GetButton("Jump"));
        _input.Buttons.Set(MyButtons.Shooting, Input.GetButtonDown("Fire1"));
        _input.Buttons.Set(MyButtons.Crouch, Input.GetButton("Crouch"));

        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.delta.ReadValue().magnitude > 0.02f)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            Vector2 lookRotationDelta = new(-mouseDelta.y, mouseDelta.x);
            _input.AimDirection += lookRotationDelta; 
        }

    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(_input);
        _resetInput = true; 
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
        Vector3 bestPosition = Vector3.zero;
        float maxMinDistance = 0f;

        foreach (var t in transforms)
        {
            bool isFarEnough = true;
            float minDistance = float.MaxValue;

            foreach (var p in players)
            {
                float sqrDist = (t.position - p.transform.position).sqrMagnitude;
                if (sqrDist < (30 * 30f))
                {
                    isFarEnough = false;
                }
                minDistance = Mathf.Min(minDistance, sqrDist);
            }

            if (isFarEnough)
            {
                return t.position;
            }

            if (minDistance > maxMinDistance)
            {
                maxMinDistance = minDistance;
                bestPosition = t.position;
            }
        }

        return bestPosition;
    }

}