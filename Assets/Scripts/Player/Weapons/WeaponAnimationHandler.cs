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
            _gunAnim.SetTrigger("idle");
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
        if(!Runner.IsServer)
        {
            return; 
        }
        if (GetInput(out NetworkInputData data))
        {
            bool isShooting = data.Buttons.IsSet(MyButtons.Shooting);
            if (isShooting && _retriggerGunAnim && _gun.AmmoInMag > 0 && _retriggerReloadDelay)
            {
                Debug.Log("your mom"); 
                _gunAnim.SetBool("Shoot", true);
                StartCoroutine(FireDelay());
            } 
            if (data.Buttons.IsSet(MyButtons.Reload) && _retriggerReloadDelay && _gun.AmmoInMag < _gun.MagSize)
            {
                Debug.Log("Reload..."); 
                StartCoroutine(ReloadDelay());
            }
            if(!isShooting && _retriggerReloadDelay)
            {
                _gunAnim.SetBool("Shoot", false);
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
        _gunAnim.SetBool("Reloading", true);
        yield return new WaitForSeconds(_gun.ReloadDelay);
        Debug.Log("Set reload false"); 
        _retriggerReloadDelay = true;
        _gunAnim.SetBool("Reloading", false); 
    }
}
