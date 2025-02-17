using Fusion;
using Fusion.Addons.SimpleKCC;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks
{
    private bool _resetInput;
    private Vector2Accumulator _accumulator = new Vector2Accumulator(0.02f, true);
    private NetworkInputData _input = new NetworkInputData();

    void IBeforeUpdate.BeforeUpdate()
    {
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
        }

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        _input.AimDirection = _accumulator.ConsumeTickAligned(runner);
        input.Set(_input);
        _resetInput = true;
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
