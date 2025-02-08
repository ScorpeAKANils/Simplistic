using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Crouch : NetworkBehaviour 
{
    private Vector3 _originalScale;
    private Vector3 _crouchScale;
    private SimpleKCC _cc; 
    public override void Spawned()
    {
        _originalScale = transform.localScale;
        _crouchScale = _originalScale / 2; 
        _cc = this.GetComponent<SimpleKCC>();
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false) 
        {
            return; 
        }
        if(GetInput(out NetworkInputData data)) 
        {
            //crouch logic...
            if(data.Buttons.IsSet(MyButtons.Crouch)) 
            {
                Rpc_SetScale(_crouchScale);
            } else 
            {
                Rpc_SetScale(_originalScale);
            } 
        }
        else
        {
            Rpc_SetScale(_originalScale);
        }
    }

    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    public void Rpc_SetScale(Vector3 scale) 
    {
        Rpc_Crouch(scale); 
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Crouch(Vector3 scale) 
    {
        transform.localScale = scale;
    }
}
