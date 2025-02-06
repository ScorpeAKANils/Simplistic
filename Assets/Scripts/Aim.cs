using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] private float _normalFov = 90f;
    [SerializeField] private List<float> _aimFov = new(); 
    [SerializeField] private WeaponManager _wM;
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
        _isAiming = Input.GetButton("Fire2"); 
        if(_isAiming) 
        {
            _cam.fieldOfView = _aimFov[_wM.CurrentWeapon];
            _mL.SetSensitivityFactor((_aimFov[_wM.CurrentWeapon] / _normalFov) * 0.5f); 
            _wM.GetActiveWeapon().Anim.SetBool("Aim", true);
        } else 
        {
            _cam.fieldOfView= _normalFov;
            _mL.SetSensitivityFactor(_aimFov[_wM.CurrentWeapon] / _normalFov);
            _wM.GetActiveWeapon().Anim.SetBool("Aim", false);
        }
    }
}
