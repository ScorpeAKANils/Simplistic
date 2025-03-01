using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 

public class Settings : NetworkBehaviour
{
    public AudioMixer audioMixer;
    private InputManager _inputManager;
    private void Start()
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(0.25f) * 20);
        _inputManager = FindObjectOfType<InputManager>(); 
    }
    // Setzt die Lautstärke des Master-AudioMixers
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        _inputManager.SetSensitivity(sensitivity);
    }
}
