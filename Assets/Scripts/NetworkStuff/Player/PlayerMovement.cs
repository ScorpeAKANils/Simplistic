using Fusion;
using Fusion.Addons.SimpleKCC; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 55f;
    private SimpleKCC _cc;
    private Vector3 _moveVector;
    private float _jumpImpulse = 4f;

    private void Awake()
    {
        _cc = GetComponent<SimpleKCC>();
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _moveVector = ReadInput(data); 
            float jump = 0;
            if (data.Buttons.IsSet(MyButtons.Jump) && _cc.IsGrounded) 
            {
                jump = _jumpImpulse; 
            }
            _cc.Move(_moveVector.normalized * _speed, jump); 
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
