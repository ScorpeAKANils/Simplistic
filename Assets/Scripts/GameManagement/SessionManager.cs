using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Fusion.Sockets;
using System;

public class SessionManager : SimulationBehaviour
{

    public static async Task  ConnectToLobby(NetworkRunner runner) 
    {
        if(runner == null) 
        {
            throw new System.NullReferenceException("There is no runner to work with"); 
        }
        var result = await runner.JoinSessionLobby(SessionLobby.ClientServer); 
        if(result.Ok) 
        {
            Debug.Log("Succesfully joined a Lobby");
        } else 
        {
            Debug.LogError("We are sorry to Inform you, that you couldnt join a lobby." + result.ShutdownReason); 
        } 
    }

    public static async Task CreateSession(NetworkRunner runner, string sessionName, SceneRef scene, GameObject gameObject) 
    {
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            CustomLobbyName = sessionName,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("Succesfully created seassion"); 
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public static async Task JoinLobby(NetworkRunner runner, string sessionName, GameObject gameObject)
    {

        var result = await runner.JoinSessionLobby(SessionLobby.Custom, sessionName);
       
        if (result.Ok)
        {
            Debug.Log("Succesfully joined seassion");
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
        var task = JoinGame(runner, sessionName, gameObject);

    }

    public static Task CreateGame(NetworkRunner runner, string sessionName, SceneRef scene, GameObject gameObject) 
    {
        return InitNetworkRunner(runner, GameMode.Host, sessionName, runner.GetPlayerConnectionToken(), NetAddress.Any(), scene, null, gameObject); 
    }

    public static Task JoinGame(NetworkRunner runner, string sessionName, GameObject gameObject) 
    {
        SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex); 
        var task = InitNetworkRunner(runner, GameMode.Client, sessionName, runner.GetPlayerConnectionToken(), NetAddress.Any(), scene, null, gameObject);
        return task; 
    }
    protected static Task InitNetworkRunner(NetworkRunner runner, GameMode mode, string sessionName,byte[] connectionToken, NetAddress address, SceneRef scene, Action<NetworkRunner> nAction, GameObject gameObject) 
    {
        var sceneManager = runner.SceneManager;
        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            Address = address,
            Scene = scene,
            CustomLobbyName = sessionName,
            SessionName = sessionName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            ConnectionToken = connectionToken
        }); 
    }
}
