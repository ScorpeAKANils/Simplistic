using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;

    [SerializeField] private MouseLook _mL; 
    private void Start()
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(0.25f) * 20);
    }
    // Setzt die Lautstärke des Master-AudioMixers
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        _mL.SetSensitivity(sensitivity);
    }
}
