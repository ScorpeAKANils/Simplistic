using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSettings : MonoBehaviour
{
    private GameObject _settings;
    private bool _settingsActive = false;
    private bool _cursorLocked;

    // Start is called before the first frame update
    void Start()
    {
        _settings = FindObjectOfType<SettingsTag>().gameObject;
        ToggleMouseState();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_settingsActive)
        {
            _settingsActive = true;
            ToggleMouseState();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && _settingsActive)
        {
            _settingsActive = false;
            ToggleMouseState();
        }
        SettingsTag.Instance.ToggleSettings(_settingsActive); 
    }

    private void ToggleMouseState()
    {
        if (_cursorLocked == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        _cursorLocked = !_cursorLocked;
        Cursor.visible = !_cursorLocked;
    }
}
