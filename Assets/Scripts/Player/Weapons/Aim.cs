using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : NetworkBehaviour
{
    [SerializeField] private float _normalFov = 90f;
    [SerializeField] private float _aimFov;
    [SerializeField] private Weapon _wM;
    [SerializeField] private Animator _controller;
    [SerializeField] private MouseLook _mL;
    [SerializeField] private Health _health;
    private InputManager _inputManager; 
    private Camera _cam;
    void Start()
    {
        _cam = Camera.main;
        _cam.fieldOfView = _normalFov;
        _inputManager = FindObjectOfType<InputManager>(); 
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        try
        {
            if (GetInput(out NetworkInputData data))
            {
                if (data.Buttons.IsSet(MyButtons.Aim) && HasInputAuthority)
                {
                    _cam.fieldOfView = _aimFov;
                    _controller.SetBool("Aim", true);
                    _inputManager.SetSensitivityFactor(_aimFov / _normalFov);
                }
                else 
                {
                    _cam.fieldOfView = _normalFov;
                    _controller.SetBool("Aim", false);
                    _inputManager.SetSensitivityFactor(1);
                }
            }
        }
        catch
        {
        }
    }
}
