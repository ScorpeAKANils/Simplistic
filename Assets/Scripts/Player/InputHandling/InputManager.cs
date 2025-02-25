using Fusion;
using Fusion.Addons.SimpleKCC;
using Fusion.Addons.KCC;  
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks
{
    private bool _resetInput;
    private Fusion.Addons.SimpleKCC.Vector2Accumulator _accumulator = new Fusion.Addons.SimpleKCC.Vector2Accumulator(0.02f, true);
    private Fusion.Addons.KCC.SmoothVector2 _smoother = new(128);
    private NetworkInputData _input = new NetworkInputData();
    private PlayerInputActionMaps _inputActions; 

    void Awake()
    {
        _inputActions = new PlayerInputActionMaps();
        _inputActions.Player.Enable();
    }
    public void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //_input.AimDirection = _accumulator.ConsumeTickAligned(runner);
        input.Set(_input);
        _resetInput = true;
    }


    public void BeforeUpdate() 
    {
        if (_resetInput)
        {
            _resetInput = false;
            _input = default;
        }
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        //Movement
        _input.MoveDirection = _inputActions.Player.Movement.ReadValue<Vector2>();
        Vector2 mouseDelta = Vector2.zero;  //= _inputActions.Player.MouseLook.ReadValue<Vector2>();
        Mouse mouse = Mouse.current;
        Gamepad gamepad = Gamepad.current;
        if (mouse != null && gamepad == null) 
        {
            mouseDelta = mouse.delta.ReadValue();
            
        } else if(gamepad != null)
        {
            mouseDelta = gamepad.rightStick.ReadValue() * 15f;
        }
        _smoother.AddValue(Time.frameCount, Time.unscaledDeltaTime, mouseDelta);
        Vector2 smoothedVal = _smoother.CalculateSmoothValue(0.01, Time.unscaledDeltaTime); 
        /*Vector2 lookRotationDelta*/ _input.AimDirection = new(-smoothedVal.y, smoothedVal.x);
        //_accumulator.Accumulate(lookRotationDelta);
        _input.Buttons.Set(MyButtons.Jump, _inputActions.Player.Jump.IsPressed());
        _input.Buttons.Set(MyButtons.Crouch, _inputActions.Player.Crouch.IsPressed());

        //Weapons
        _input.Buttons.Set(MyButtons.Shooting, _inputActions.Player.Shoot.IsPressed());
        _input.Buttons.Set(MyButtons.Reload, _inputActions.Player.Reload.IsPressed());
        _input.Buttons.Set(MyButtons.Aim, _inputActions.Player.Aim.IsPressed());
        _input.Buttons.Set(MyButtons.Protogun, _inputActions.Player.ProtoGun.IsPressed());
        _input.Buttons.Set(MyButtons.SilentDeath, _inputActions.Player.SilentDeath.IsPressed());
        _input.Buttons.Set(MyButtons.ShowScoreBoard, Input.GetKey(KeyCode.Tab));
    }

    //this shit needs to be here, because of the INetworkRunnerCallbacks, because Fusion gives a shit about solid i guess 
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
}
