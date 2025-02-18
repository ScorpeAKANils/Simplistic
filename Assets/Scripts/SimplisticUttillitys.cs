using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplisticUttillitys : MonoBehaviour
{
    /// <summary>
    /// Checks if the Raycast hit another Player. If so, it Applys Damage and returns true. 
    /// If not, it returns false. Uses Try Catch to prevent Nullreferences that would stop the execution on Fusion side. 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="damage"></param>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool CheckForHit(Hitbox obj, float damage, PlayerRef self)
    {
        try 
        {
            Health health = obj.GetComponent<Health>();
            ApplyDamage(health, damage, self); 
            return true; 
        }
        catch 
        {
            return false;  
        }
    }

    private static void ApplyDamage(Health h, float damage, PlayerRef self) 
    {
        PlayerRef target = h.GetPlayer();
        h.DealDamage(target, damage, self);
    }
}
