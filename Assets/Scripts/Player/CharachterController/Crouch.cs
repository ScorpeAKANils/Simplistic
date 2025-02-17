using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Crouch : NetworkBehaviour 
{
    [SerializeField] private Hitbox _hitBox; 
    private Vector3 _originalScale;
    private Vector3 _hitBoxOSize;
    private Vector3 _hitBoxCrouchSize;
    private float _hitBoxOffsetY;
    private float _hitBoxOffsetYCrouch; 
    private Vector3 _crouchScale;
    private SimpleKCC _cc; 
    public override void Spawned()
    {
        _originalScale = transform.localScale;
        _crouchScale = _originalScale * 0.75f;
        _hitBoxOSize = _hitBox.BoxExtents;
        _hitBoxCrouchSize = _hitBoxOSize * 0.75f; 
        _hitBoxOffsetY = _hitBox.Offset.y;
        _hitBoxOffsetYCrouch = _hitBoxOffsetY * 0.75f;
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
                Rpc_SetScale(_crouchScale, _hitBoxCrouchSize, _hitBoxOffsetYCrouch);
            } else 
            {
                Rpc_SetScale(_originalScale, _hitBoxOSize, _hitBoxOffsetY);
            } 
        }
        else
        {
            Rpc_SetScale(_originalScale, _hitBoxOSize, _hitBoxOffsetY);
        }
    }

    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    public void Rpc_SetScale(Vector3 scale, Vector3 hitBoxSize, float offset) 
    {
        Rpc_Crouch(scale, hitBoxSize, offset); 
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Crouch(Vector3 scale, Vector3 hitBoxSize, float offset) 
    {
        transform.localScale = scale;
        _hitBox.BoxExtents = hitBoxSize;
        _hitBox.Offset.y = offset; 
    }
}
