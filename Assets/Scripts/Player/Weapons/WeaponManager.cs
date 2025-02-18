using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    public List<Weapon> Weapons = new();
    [Networked] public int CurrentWeapon { get; set; }
    [Networked, Capacity(2)] public NetworkDictionary<WeaponType, bool> WeaponsCollectionStatus => default; 
    public int oldWeapon = 0;

    public void ActivateWeapon(WeaponType weapon)
    {
        for (int i = 0; i < Weapons.Count; i++)
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

    public Weapon GetActiveWeapon() 
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
        if(Runner.IsServer) 
        {
            if(GetInput(out NetworkInputData data)) 
            {
                if(data.Buttons.IsSet(MyButtons.Protogun)) 
                {
                    SetWeaponActive(WeaponType.Protogun);
                }

                if (data.Buttons.IsSet(MyButtons.SilentDeath))
                {
                    SetWeaponActive(WeaponType.Sniper); 
                }
                ActivateWeapon((WeaponType)CurrentWeapon); 
            }
        }
    }

    public void SetWeaponActive(WeaponType type) 
    {
        if (WeaponsCollectionStatus[type] == true | (int)type == 0) 
        {
            CurrentWeapon = (int)type;
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
    public Weapon GetWeapon(WeaponType weapon)
    {
        return Weapons[(int)weapon]; 
    }
}
