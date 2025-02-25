using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsTag : MonoBehaviour
{
    [SerializeField]
    private GameObject Settings;

    public void ToggleSettings(bool val)
    {
        Settings.SetActive(val); 
    }
}
