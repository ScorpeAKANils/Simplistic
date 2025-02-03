using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : NetworkBehaviour 
{
    private Vector3 _originalScale;
    private Vector3 _crouchScale;

    public override void Spawned()
    {
        _originalScale = transform.localScale;
        _crouchScale = _originalScale / 2; 
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data)) 
        {
            //crouch logic...
            if(data.Buttons.IsSet(MyButtons.Crouch)) 
            {
                transform.localScale = _crouchScale; 
            } else 
            {
                transform.localScale = _originalScale;
            } 
        }
        else
        {
            transform.localScale = _originalScale;
        }
    }
}
