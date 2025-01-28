using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 5f;
    
    private NetworkCharacterController _cc;
    private Vector3 _moveVector;
    float yVel = 0f; 

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(_speed * TranslateDirectionToLocalSpace(data.direction) * Runner.DeltaTime);
        }
    }

    private Vector3 TranslateDirectionToLocalSpace(Vector3 dir) 
    {
        Vector3 moveVector = Vector3.zero;
        if(dir.z != 0) 
        {
            moveVector += transform.forward * dir.z;  
        }

        if(dir.x != 0)
        {
            moveVector += transform.right * dir.x; 
        }

        return moveVector.normalized; 

    }
}
