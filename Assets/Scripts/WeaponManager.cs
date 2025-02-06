using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    public List<FireBullet> _weapons = new();
    [Networked] public int CurrentWeapon { get; set; }
    public int oldWeapon = 0;

    public void ActivateWeapon(WeaponType weapon)
    {
        for (int i = 0; i < _weapons.Count; i++)
        {
            if (i == (int)weapon)
            {
                Rpc_SetWeaponActive(i, true); 
            }
            else
            {
                Rpc_SetWeaponActive(i, false);
            }
        }
    }

    public FireBullet GetActiveWeapon() 
    {
        return _weapons[CurrentWeapon];     
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (oldWeapon != CurrentWeapon)
        {
            oldWeapon = CurrentWeapon;
            ActivateWeapon((WeaponType)CurrentWeapon);
        }

        for (int i = 0; i <= _weapons.Count-1; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha0 + i)))
            {
                    Rpc_InformOverWeaponChange(i); 
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)] 
    public void Rpc_InformOverWeaponChange(int newWeapon) 
    {
            Rpc_SetWeaponCurrent(newWeapon, this); 
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetWeaponCurrent(int weapon, WeaponManager wM)
    {
        if (Runner.IsServer == false) 
        {
            return; 
        }
        if (wM._weapons[weapon].IsCollected | weapon == 0)
            wM.CurrentWeapon = weapon; 
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetWeaponState(int weapon, bool val)
    {
        Rpc_SetWeaponActive(weapon, true);
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetWeaponActive(int weapon, bool val) 
    {
        _weapons[weapon].gameObject.SetActive(val);
    }
    public FireBullet GetWeapon(WeaponType weapon)
    {
        return _weapons[(int)weapon]; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
