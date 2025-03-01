using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 

public class Settings : NetworkBehaviour
{
    public AudioMixer audioMixer;

    [SerializeField] private MouseLook _mL;
    [SerializeField] private Health _health; 
    private void Start()
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(0.25f) * 20);
    }
    // Setzt die Lautstärke des Master-AudioMixers
    public void SetMasterVolume(float volume)
    {
        if(Runner.LocalPlayer == _health.GetPlayer())
            audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        if (Runner.LocalPlayer == _health.GetPlayer())
            _mL.SetSensitivity(sensitivity);
    }
}
