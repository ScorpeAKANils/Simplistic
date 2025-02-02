using UnityEngine;
using Fusion; 

public class SetCamera : SimulationBehaviour
{
    [SerializeField] private Health _playerRef; 
    
    private Transform _camPos;
    private Camera _cam;
    private BasicSpawner _spawner;
    private bool _hasBeenPlaceced = false; 
    private void Start()
    {
        _spawner = FindObjectOfType<BasicSpawner>();
        if (_spawner == null) 
        {
            throw new System.NullReferenceException("Spawner not found..."); 
        }
    }


    private void Update()
    {
        if (_cam == null) 
        {
            if(_playerRef.GetPlayer() == _spawner.RunnerRef.LocalPlayer && !_hasBeenPlaceced) 
            {
                _hasBeenPlaceced = true; 
                _camPos = _playerRef.GetComponentInChildren<TagCamPos>().transform; 
                _cam = FindObjectOfType<Camera>();
                _cam.transform.position = _camPos.position; 
                _cam.transform.transform.rotation = _camPos.rotation;
                _cam.transform.parent = _camPos.transform; 
            }
             
         }
    }
}