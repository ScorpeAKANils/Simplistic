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
    ShowScoreBoard
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector2 AimDirection; 
}
