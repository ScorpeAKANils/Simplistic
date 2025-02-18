using Fusion;
using UnityEngine;

enum MyButtons 
{
    Forward, 
    Backward, 
    Left, 
    Right, 
    Jump, 
    Shooting, 
    Crouch, 
    Slide,
    ShowScoreBoard,
    Reload, 
    Protogun, 
    SilentDeath
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector2 AimDirection; 
}
