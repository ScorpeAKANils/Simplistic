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
    Slide
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector2 AimDirection; 
}
