using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] private float _normalFov = 90f;
    [SerializeField] private float _aimFov; 
    [SerializeField] private Weapon _wM;
    [SerializeField] private MouseLook _mL; 
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
        try 
        {
             _isAiming = Input.GetButton("Fire2"); 
             if(_isAiming) 
             {
                 _cam.fieldOfView = _aimFov;
                 _wM.Anim.SetBool("Aim", true);
                _mL.SetSensitivityFactor(_aimFov/_normalFov);
            } else 
             {
                 _cam.fieldOfView= _normalFov;
                 _wM.Anim.SetBool("Aim", false);
                _mL.SetSensitivityFactor(1); 
             }
        } 
        catch 
        {

        }
    }
}
