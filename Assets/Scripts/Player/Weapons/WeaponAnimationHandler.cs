using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)] // Dieses Script wird früher ausgeführt
public class WeaponAnimationHandler : NetworkBehaviour
{
    [SerializeField] private Weapon _gun;
    [SerializeField] private NetworkedAnimationController _gunAnim;
    private bool _retriggerGunAnim = true;
    private bool _retriggerReloadDelay = true;
    private bool _wasEnabled = false; 


    public void OnEnable()
    {
        if (_wasEnabled) 
        {
            _gunAnim.Rpc_SetTrigger("idle");
        }
        _wasEnabled = true; 
        _retriggerGunAnim = true;
        _retriggerReloadDelay = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            return;
        }
        if (GetInput(out NetworkInputData data))
        {

            if (data.Buttons.IsSet(MyButtons.Shooting) && _retriggerGunAnim && _gun.AmmoInMag > 0 && _retriggerReloadDelay)
            {
                _gunAnim.Rpc_SetTrigger("Shoot");
                StartCoroutine(FireDelay());
            }

            if (data.Buttons.IsSet(MyButtons.Reload) && _retriggerGunAnim && _gun.AmmoInMag < _gun.MagSize)
            {
                _gunAnim.Rpc_SetTrigger("Reload");
                StartCoroutine(ReloadDelay());
            }
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
