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
    private Vector2 _mouseVec; 
 
    public Transform CamTransform { get { return _camTransform; } }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer(Runner.LocalPlayer, out NetworkInputData data) /*&& HasInputAuthority*/)
        {
            _mouseVec = data.AimDirection * _sensitivityFactor; 
        }
        MoveCamera(); 
    }

    private void MoveCamera() 
    {
        if (HasInputAuthority)
        {
            //_cc.AddLookRotation(data.AimDirection * _sensitivityFactor, -89f, 89);
            _cc.AddLookRotation(_mouseVec); 
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
         MoveCamera();
    }
    public void LateUpdate()
    {
        MoveCamera(); 
    }
    public void RefreshCamera()
    {
        if(HasInputAuthority) 
        {
            Vector2 pitchRotation = _cc.GetLookRotation(true, false);
            _camTransform.localRotation = Quaternion.Euler(pitchRotation);
        }
    }
}