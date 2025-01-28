using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _sensitivity = 50f;
    private bool _cursorLocked = false;
    private float mouseX;
    private float _xRotation; 
    [SerializeField] private Transform _camTransform; 

    // Update is called once per frame

    private void Start()
    {
        ToggleMouseState();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) 
        {
            ToggleMouseState(); 
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (GetInput(out NetworkInputData data))
        {
            _xRotation -= (data.MouseX * _sensitivity * Runner.DeltaTime); 
            _xRotation = Mathf.Clamp(_xRotation, -90, 90);
            _camTransform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            transform.Rotate((transform.up) * data.MouseY * _sensitivity * Runner.DeltaTime); 
        }
    }

    private void ToggleMouseState() 
    {
        if(_cursorLocked == false) 
        {
            Cursor.lockState = CursorLockMode.Locked;
        } else 
        {
            Cursor.lockState = CursorLockMode.None;
        }
        _cursorLocked = !_cursorLocked;
        Cursor.visible = !_cursorLocked;
    }
}
