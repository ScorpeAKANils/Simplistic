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

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (oldWeapon != CurrentWeapon)
        {
            oldWeapon = CurrentWeapon;
            ActivateWeapon((WeaponType)CurrentWeapon);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
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
