using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    public List<FireBullet> Weapons = new();
    [Networked] public int CurrentWeapon { get; set; }
    [Networked, Capacity(2)] public NetworkDictionary<WeaponType, bool> WeaponsCollectionStatus => default; 
    public int oldWeapon = 0;

    public void ActivateWeapon(WeaponType weapon)
    {
        for (int i = 0; i < Weapons.Count; i++)
        {
            if (i == (int)weapon)
            {
                Weapons[i].ResetCanFire(); 
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
        return Weapons[CurrentWeapon];     
    }

    public void ResetWeaponCollectionStatus() 
    {
        CurrentWeapon = 0; 
        //i = 1, beause the basic gun is always collected.  
        for(int i = 1; i <= Weapons.Count-1; i++) 
        {
            WeaponsCollectionStatus.Set((WeaponType)i, false); 
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (oldWeapon != CurrentWeapon)
        {
            oldWeapon = CurrentWeapon;
            ActivateWeapon((WeaponType)CurrentWeapon);
        }

        for (int i = 0; i <= Weapons.Count-1; i++)
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
        if (wM.WeaponsCollectionStatus[(WeaponType)weapon] == true | weapon == 0)
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
        Weapons[weapon].gameObject.SetActive(val);
    }
    public FireBullet GetWeapon(WeaponType weapon)
    {
        return Weapons[(int)weapon]; 
    }
}
