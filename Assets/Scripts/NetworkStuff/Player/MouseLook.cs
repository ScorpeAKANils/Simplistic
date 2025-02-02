using Fusion;
using Fusion.Addons.SimpleKCC; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] private float _sensitivity = 30f;
    [SerializeField] private Transform _camTransform;
    [SerializeField] FireBullet _bullet;
    [SerializeField] private SimpleKCC _cc;

    public override void FixedUpdateNetwork()
    { 
        MoveCamera(); 
    }

    private void MoveCamera() 
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector2 mouseDir = data.AimDirection;
            mouseDir.y += _bullet.GetYRecoile(data); 
            _cc.AddLookRotation(mouseDir * _sensitivity); 
            _camTransform.localRotation = Quaternion.Euler(_cc.GetLookRotation().x + _bullet.GetXRecoile(data), 0, 0);
        }
    }

    public override void Render()
    {
        base.Render();
        MoveCamera(); 
    }
}