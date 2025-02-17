using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsTag : MonoBehaviour
{
    [SerializeField]
    private GameObject Settings;
    public static SettingsTag Instance; 
    // Start is called before the first frame update
    void Start()
    {
        Instance = this; 
    }

    public void ToggleSettings(bool val)
    {
        Settings.SetActive(val); 
    }
}
