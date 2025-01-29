using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 Direction;
    public float MouseY;
    public float MouseX;
    public bool Fired; 
    public bool Jump; 

}
