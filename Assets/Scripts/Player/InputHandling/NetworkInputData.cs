using Fusion;
using UnityEngine;

enum MyButtons 
{
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
    public Vector2 MoveDirection; 
}
