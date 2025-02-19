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
            Vector2 mouseDir = data.AimDirection;
            _cc.AddLookRotation(mouseDir);
            RefreshCamera(); 
        }
    }

    public void SetSensitivityFactor(float val) 
    {
        if(HasInputAuthority)
            _sensitivityFactor = val; 
    }

   public override void Render()
   {
        RefreshCamera();
   }

    public void LateUpdate()
    {
        RefreshCamera();
    }

    public void RefreshCamera()
    {
        Vector2 pitchRotation = _cc.GetLookRotation(true, false);
        _camTransform.localRotation = Quaternion.Euler(pitchRotation);
    }
}