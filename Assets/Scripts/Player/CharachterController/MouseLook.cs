using Fusion;
using Fusion.Addons.SimpleKCC; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] private float _sensitivityFactor = 1; 
    [SerializeField] private Transform _camTransform;
    [SerializeField] private List<Weapon> _bullet = new();
    [SerializeField] private WeaponManager _wM;
    [SerializeField] private SimpleKCC _cc;

    public Transform CamTransform { get { return _camTransform; } }

    public override void FixedUpdateNetwork()
    { 
        MoveCamera(); 
    }

    private void MoveCamera() 
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector2 mouseDir = data.AimDirection * _sensitivityFactor;
            _cc.AddLookRotation(mouseDir, -89f, 89f); 
            _camTransform.localRotation = Quaternion.Euler(_cc.GetLookRotation().x, 0, 0);
        }
    }

    public void SetSensitivityFactor(float val) 
    {
        if(HasInputAuthority)
            _sensitivityFactor = val; 
    }

   public override void Render()
   {
       MoveCamera(); 
   }
}