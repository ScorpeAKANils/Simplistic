using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationHandler : SimulationBehaviour
{
    [SerializeField] private Weapon _gun;
    [SerializeField] private Animator _gunAnim;
    private bool _retriggerGunAnim = true;
    private bool _retriggerReloadDelay = true; 

    public void OnEnable()
    {
        _gunAnim.SetTrigger("idle");
        _retriggerGunAnim = true;
        _retriggerReloadDelay = true;
    }
    public void Update()
    {
        if(_gun.HasInputAuthority==false | Cursor.lockState == CursorLockMode.None) 
        {
            return; 
        }
        if (Input.GetButton("Fire1") && _retriggerGunAnim && _gun.AmmoInMag > 0 && _retriggerReloadDelay )
        {
            _gunAnim.SetTrigger("Shoot");
            StartCoroutine(FireDelay());
        }

        if (Input.GetKeyDown(KeyCode.R) && _retriggerGunAnim && _gun.AmmoInMag < _gun.MagSize)
        {
            _gunAnim.SetTrigger("Reload");
            StartCoroutine(ReloadDelay());
        }
    }

    private IEnumerator FireDelay() 
    {
        _retriggerGunAnim = false; 
        yield return new WaitForSeconds(_gun.FireDelay);
        _retriggerGunAnim = true;
    }

    private IEnumerator ReloadDelay()
    {
        _retriggerReloadDelay = false;
        yield return new WaitForSeconds(_gun.ReloadDelay);
        _retriggerReloadDelay = true;
    }
}
