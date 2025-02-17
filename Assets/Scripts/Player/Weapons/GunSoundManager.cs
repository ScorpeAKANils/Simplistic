using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSoundManager : NetworkBehaviour
{
    [SerializeField] private AudioSource _audio;


    [Rpc(RpcSources.All, RpcTargets.All)] 
    public void Rpc_PlayShotSound() 
    {
        _audio.Play(); 
    }
}
