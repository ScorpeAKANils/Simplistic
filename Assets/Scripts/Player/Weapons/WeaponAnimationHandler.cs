using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationHandler : MonoBehaviour
{
    [SerializeField] private Weapon _gun;
    [SerializeField] private Animator _gunAnim;
    private bool retriggerAnim = true;

    public void OnEnable()
    {
        retriggerAnim = true;
    }
    public void Update()
    {
        if (Input.GetButton("Fire1") && retriggerAnim)
        {
            _gunAnim.SetTrigger("Shoot");
            StartCoroutine(Delay());
        }       
    }

    private IEnumerator Delay() 
    {
        retriggerAnim = false; 
        yield return new WaitForSeconds(_gun.FireDelay);
        retriggerAnim = true;
    }
}
