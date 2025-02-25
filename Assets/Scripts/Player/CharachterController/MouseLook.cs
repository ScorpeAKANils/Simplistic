using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] private float _sensitivityFactor = 1;
    [SerializeField] private float _sensitivity = 10f; 
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
            Vector2 mouseDir = data.AimDirection * ((_sensitivity/64f) * _sensitivityFactor);
            _cc.AddLookRotation(mouseDir, -89f, 89f);
            _camTransform.localRotation = Quaternion.Euler(_cc.GetLookRotation().x, 0, 0);
        }
    }

    public void SetSensitivityFactor(float val)
    {
        if(!HasInputAuthority) 
        {
            return; 
        }
        _sensitivityFactor = val;
    }

    public void SetSensitivity(float val) 
    {
        if (!HasInputAuthority)
        {
            return;
        }
        _sensitivity = val; 
    }

    public override void Render()
    {
        MoveCamera();
    }
}