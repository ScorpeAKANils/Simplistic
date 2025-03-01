using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] private Transform _camTransform;
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
            _cc.AddLookRotation(data.AimDirection, -89f, 89f);
            _camTransform.localRotation = Quaternion.Euler(_cc.GetLookRotation().x, 0, 0);
        }
    }

    public override void Render()
    {
        MoveCamera();
    }
}