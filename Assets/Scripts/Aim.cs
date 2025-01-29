using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] private float _normalFov = 90f;
    [SerializeField] private float _aimFov = 50f;
    private bool _isAiming; 
    private Camera _cam; 
    void Start()
    {
        _cam = Camera.main;
        _cam.fieldOfView = _normalFov;
    }

    // Update is called once per frame
    void Update()
    {
        _isAiming = Input.GetButton("Fire2"); 
        if(_isAiming) 
        {
            _cam.fieldOfView = _aimFov; 
        } else 
        {
            _cam.fieldOfView= _normalFov;
        }
    }
}
