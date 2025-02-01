using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _sensitivityX = 10f;
    [SerializeField] private float _sensitivityY = 30f;
    private bool _cursorLocked = false;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private NetworkCharacterController _cc;
    [SerializeField] FireBullet _bullet; 
    private float _xRotation;
    private float _yRotation;
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
        if (!HasInputAuthority)
        {
            return; 
        }
        if (GetInput(out NetworkInputData data))
        {
            _xRotation -= (data.AimDirection.x + _bullet.GetXRecoile(data)) * (_sensitivityX / 2) * Runner.DeltaTime;
            _yRotation = data.AimDirection.y + _bullet.GetYRecoile(data); 
            MoveCamera(); 
        }
    }

    private void MoveCamera() 
    {
        _xRotation = Mathf.Clamp(_xRotation, -80, 80);
        _camTransform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        transform.Rotate(Vector3.up, _yRotation * _sensitivityY * Runner.DeltaTime); 
    }

    public override void Render()
    {
        MoveCamera(); 
    }

    private float GetPingFactor() 
    {
        float latency = (float)Runner.GetPlayerRtt(Runner.LocalPlayer) * 1000f;
        return Mathf.Clamp(1 - (latency / 200), 0.5f, 1f); 
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
