using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSoundManager : NetworkBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private Weapon _gun;
    private bool _retriggerSound = true;


    public void OnEnable()
    {
        _retriggerSound = true;
    }
    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data) && _gun.AmmoInMag > 0) 
        {
            if(_gun.PlayerFiresGun && _retriggerSound)
            {
                _audio.Play();
                StartCoroutine(Delay()); 
            }
        }
    }

    private IEnumerator Delay()
    {
        _retriggerSound = false;
        yield return new WaitForSeconds(_gun.FireDelay);
        _retriggerSound = true;
    }
}
