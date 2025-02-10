using Fusion;
using Fusion.Addons.SimpleKCC; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] private float _sensitivityFactor = 1; 
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
            mouseDir.x -= _bullet[_wM.CurrentWeapon].GetXRecoile(data); 
            _cc.AddLookRotation(mouseDir, -89f, 89f); 
            _camTransform.localRotation = Quaternion.Euler(_cc.GetLookRotation().x, 0, 0);
        }
    }

    public void SetSensitivityFactor(float val) 
    {
        _sensitivityFactor = val; 
    }

   public override void Render()
   {
       base.Render();
       MoveCamera(); 
   }
}