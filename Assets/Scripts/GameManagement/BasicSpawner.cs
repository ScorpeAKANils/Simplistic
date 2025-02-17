using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using Fusion.Addons.SimpleKCC;
using System.Linq;

public class BasicSpawner : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private NetworkPrefabRef _roundManager;
    [SerializeField] private NetworkPrefabRef _kdManager; 
    [SerializeField] private List<SpawnPosTag> transforms = new();
    [SerializeField] private Camera _cam;
    [SerializeField] private KdManager kdManager;
    [SerializeField] private string _sceneToLoad; 
    private Vector2Accumulator _accumulator = new Vector2Accumulator(0.02f, true);
    [SerializeField] private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();
    private Dictionary<PlayerRef, Health> _playersHealths = new(); 
    private NetworkRunner _runnerRef;
    private GameObject _mainCam; 
    public  NetworkRunner RunnerRef;
    private bool _resetInput;
    private NetworkInputData _input = new NetworkInputData();
    private TextMeshProUGUI _kdText;
    public int PlayerCount = 0; 
    
    public string RoomName = string.Empty; 


    void Start() 
    {
        Application.runInBackground = true; 
    }
    public float ReturnPlayerHealth(PlayerRef player) 
    {
        return _playersHealths[player].GetHealth(); 
    }


    public void RestartGame() 
    {
        DespawnPlayer();
        _spawnedCharacters.Clear();
        _playersHealths.Clear();
        SpawnPlayers();
    }

    public void SpawnPlayers() 
    {
        if(Runner.IsServer) 
        {
            PlayerCount = 0;
            foreach (var player in Runner.ActivePlayers)
            {
                _mainCam.gameObject.SetActive(true);
                Vector3 pos = GetRandomPos();
                NetworkObject networkPlayerObject = Runner.Spawn(_playerPrefab, pos, Quaternion.identity, player);
                if (_kdText == null)
                {
                    _kdText = FindObjectOfType<KdTagText>().GetComponent<TextMeshProUGUI>();
                }
                _spawnedCharacters.Add(player, networkPlayerObject);
                Health playerHealth = networkPlayerObject.GetComponent<Health>();
                _playersHealths.Add(player, playerHealth);
                playerHealth.SetPlayerRef(player);
                playerHealth.InitHealth();
                PlayerCount++;
                KdManager.Instance.AddPlayerToScoreBoard(player);
            }
        }
    }
    public async void StartGame(GameMode mode)
    {
        _runnerRef = GetComponent<NetworkRunner>(); 
        // Create the Fusion runner and let it know that we will be providing user input
        var networkEvents = this.gameObject.GetComponent<NetworkEvents>();
        networkEvents.OnInput.AddListener(OnInput);
        RunnerRef = _runnerRef;
        // Create the NetworkSceneInfo from the current scene
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(_sceneToLoad);
        var scene = SceneRef.FromIndex(sceneIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        // Start or join (depends on gamemode) a session with a specific name
        await _runnerRef.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = RoomName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }



    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            _mainCam = Camera.main.gameObject; 
            Vector3 pos = GetRandomPos();
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, pos, Quaternion.identity, player);
            if (_kdText == null)
            {
                _kdText = FindObjectOfType<KdTagText>().GetComponent<TextMeshProUGUI>();
            }
            _spawnedCharacters.Add(player, networkPlayerObject);
            Health playerHealth = networkPlayerObject.GetComponent<Health>();
            _playersHealths.Add(player, playerHealth);
            playerHealth.SetPlayerRef(player);
            playerHealth.InitHealth();
            PlayerCount++;
            Debug.Log(PlayerCount); 
            if(PlayerCount == 2) 
            {
                runner.Spawn(_kdManager, transform.position, Quaternion.identity);
                runner.Spawn(_roundManager, transform.position, Quaternion.identity);
                foreach(var r in runner.ActivePlayers) 
                {
                    KdManager.Instance.AddPlayerToScoreBoard(r); 
                }
            } else if(PlayerCount > 2) 
            {
                KdManager.Instance.AddPlayerToScoreBoard(player);
            }
        }
        _mainCam = Camera.main.gameObject;
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if(runner.IsServer == false)
        {
            return; 
        }
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            PlayerCount--;
            runner.Despawn(networkObject);
            var kdManagers = FindObjectsOfType<KdManager>();
            foreach (var kd in kdManagers)
            {
                kd.RemovePlayerFromScoreBoard(player);
            }
            _playersHealths.Remove(player);
            _spawnedCharacters.Remove(player);
        }
    }

    void IBeforeUpdate.BeforeUpdate()
    {
        if (_runnerRef != null)
        {
               float pingRaw =  (float)_runnerRef.GetPlayerRtt(_runnerRef.LocalPlayer) * 1000;
            int ping = Mathf.RoundToInt(pingRaw);
            if(_kdText != null) 
            {
                _kdText.text = ping.ToString(); 
            }
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
        _input.Buttons.Set(MyButtons.Shooting, Input.GetButton("Fire1"));
        _input.Buttons.Set(MyButtons.Crouch, Input.GetButton("Crouch"));
        _input.Buttons.Set(MyButtons.ShowScoreBoard, Input.GetKey(KeyCode.Tab)); 

        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            Vector2 lookRotationDelta = new(-mouseDelta.y, mouseDelta.x);
            _accumulator.Accumulate(lookRotationDelta * (7.5f * Runner.DeltaTime));
            //_input.AimDirection += lookRotationDelta; 
        }

    }

    public void DespawnPlayer() 
    {
        if (Runner.IsServer) 
        {
            foreach (var player in Runner.ActivePlayers)
            {
                Runner.Despawn(_spawnedCharacters[player]);
            }
        }   
    }

    public void HealPlayer(PlayerRef player, NetworkObject obj, ItemSpawner i) 
    {
        if (Runner.IsServer) 
        {
            _playersHealths[player].InitHealth();
            _playersHealths[player].UpdateHealthBar();
            Runner.Despawn(obj);
            i._spawned = false;
            i.Item = null; 
        }
    }
    static int _weaponType;
    static WeaponManager _wM; 
    public void CollectGun(PlayerRef player, NetworkObject obj, ItemSpawner i, WeaponType type)
    {
        if (Runner.IsServer)
        {
            _wM = _playersHealths[player].GetComponent<WeaponManager>();
            _wM.WeaponsCollectionStatus.Set(type, true);  
            Health p = _playersHealths[player];
            _wM.CurrentWeapon = (int)type; 
            Runner.Despawn(obj);
            i._spawned = false;
            i.Item = null;
        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        _input.AimDirection = _accumulator.ConsumeTickAligned(runner); 
        input.Set(_input);
        _resetInput = true; 
    }

    public static void SetIsCollectedTrue_Static()
    {
        _wM.Weapons[_weaponType].IsCollected = true;
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
        if(transforms.Count <= 0) 
        {
            transforms = FindObjectsOfType<SpawnPosTag>().ToList<SpawnPosTag>();
        }
        var players = FindObjectsOfType<PlayerMovement>();
        Vector3 bestPosition = Vector3.zero;
        float maxMinDistance = 0f;

        foreach (var t in transforms)
        {
            bool isFarEnough = true;
            float minDistance = float.MaxValue;

            foreach (var p in players)
            {
                float sqrDist = (t.transform.position - p.transform.position).sqrMagnitude;
                if (sqrDist < (100 * 100))
                {
                    isFarEnough = false;
                }
                minDistance = Mathf.Min(minDistance, sqrDist);
            }

            if (isFarEnough)
            {
                return t.transform.position;
            }

            if (minDistance > maxMinDistance)
            {
                maxMinDistance = minDistance;
                bestPosition = t.transform.position;
            }
        }

        return bestPosition;
    }

}