using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Fusion.Sockets;
using System;

public class SeassionManager : SimulationBehaviour
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

    public static async Task CreateSeassion(NetworkRunner runner, string seassionName, SceneRef scene, GameObject gameObject) 
    {
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            CustomLobbyName = seassionName,
            SessionName = seassionName,
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

    public static async Task JoinLobby(NetworkRunner runner, string seassionName, GameObject gameObject)
    {

        var result = await runner.JoinSessionLobby(SessionLobby.Custom, seassionName);
       
        if (result.Ok)
        {
            Debug.Log("Succesfully joined seassion");
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
        var task = JoinGame(runner, seassionName, gameObject);

    }

    public static Task CreateGame(NetworkRunner runner, string seassionName, SceneRef scene, GameObject gameObject) 
    {
        return InitNetworkRunner(runner, GameMode.Host, seassionName, runner.GetPlayerConnectionToken(), NetAddress.Any(), scene, null, gameObject); 
    }

    public static Task JoinGame(NetworkRunner runner, string seassionName, GameObject gameObject) 
    {
        SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex); 
        var task = InitNetworkRunner(runner, GameMode.Client, seassionName, runner.GetPlayerConnectionToken(), NetAddress.Any(), scene, null, gameObject);
        return task; 
    }
    protected static Task InitNetworkRunner(NetworkRunner runner, GameMode mode, string seassionName,byte[] connectionToken, NetAddress address, SceneRef scene, Action<NetworkRunner> nAction, GameObject gameObject) 
    {
        var sceneManager = runner.SceneManager;
        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            Address = address,
            Scene = scene,
            CustomLobbyName = seassionName,
            SessionName = seassionName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            ConnectionToken = connectionToken
        }); 
    }
}
