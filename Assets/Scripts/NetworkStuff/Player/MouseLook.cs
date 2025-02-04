using Fusion;
using Fusion.Addons.SimpleKCC; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] private float _sensitivity = 30f;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private List<FireBullet> _bullet = new();
    [SerializeField] private WeaponManager _wM;
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
            mouseDir.y += _bullet[_wM.CurrentWeapon].GetYRecoile(data); 
            _cc.AddLookRotation(mouseDir * _sensitivity); 
            _camTransform.localRotation = Quaternion.Euler(_cc.GetLookRotation().x + _bullet[_wM.CurrentWeapon].GetXRecoile(data), 0, 0);
        }
    }

    public override void Render()
    {
        base.Render();
        MoveCamera(); 
    }
}