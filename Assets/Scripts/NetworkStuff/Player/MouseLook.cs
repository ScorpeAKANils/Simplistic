using Fusion;
using Fusion.Addons.SimpleKCC; 
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
    [SerializeField] private Transform _camPos; 
    [SerializeField] FireBullet _bullet;
    [SerializeField] private SimpleKCC _cc; 
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
        MoveCamera(); 
    }

    private void MoveCamera() 
    {
        if (GetInput(out NetworkInputData data))
        {
            _xRotation -= (data.AimDirection.x + _bullet.GetXRecoile(data)) * (_sensitivityX / 2) * Runner.DeltaTime;
            _cc.AddLookRotation(data.AimDirection * _sensitivityY); 
            _camTransform.localRotation = Quaternion.Euler(_cc.GetLookRotation().x, 0, 0);
        }
    }

    public override void Render()
    {
        base.Render();
        MoveCamera(); 
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
