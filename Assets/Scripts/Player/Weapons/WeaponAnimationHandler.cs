using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)] // Dieses Script wird früher ausgeführt
public class WeaponAnimationHandler : NetworkBehaviour
{
    private bool _spawned { get; set; }    

    [SerializeField] private Weapon _gun;
    [SerializeField] private NetworkedAnimationController _gunAnim;
    private bool _retriggerGunAnim = true;
    private bool _retriggerReloadDelay = true;
    private bool _wasEnabled = false;

    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("Spawned has been called"); 
        _spawned = true; 
    }
    public void OnEnable()
    {
        if (_spawned) 
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
                _gunAnim.SetBool("Shoot", true);
                StartCoroutine(FireDelay());
            } 
            if (data.Buttons.IsSet(MyButtons.Reload) && _retriggerReloadDelay && _gun.AmmoInMag < _gun.MagSize && _retriggerGunAnim)
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
        _retriggerReloadDelay = true;
    }

    public void OnReloadEnd() 
    {
        _gunAnim.DisableBoolOnAnimationEnd("Reloading", false); 
    }

}
