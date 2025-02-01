using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 55f;
    
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
            _cc.Move(ReadInput(data) * _speed);

            if (data.Buttons.IsSet(MyButtons.Jump)) 
            {
                _cc.Jump(); 
            }
        }
    }

    private Vector3 ReadInput(NetworkInputData data) 
    {
        Vector3 moveVector = Vector3.zero;
        if(data.Buttons.IsSet(MyButtons.Forward)) 
        {
            moveVector += transform.forward;  
        } 
        else if (data.Buttons.IsSet(MyButtons.Backward)) 
        {
            moveVector += transform.forward*-1;
        }

        if (data.Buttons.IsSet(MyButtons.Right))
        {
            moveVector += transform.right;
        }
        else if (data.Buttons.IsSet(MyButtons.Left))
        {
            moveVector += transform.right * -1;
        }


        return moveVector.normalized; 

    }
}
