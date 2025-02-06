using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    [SerializeField] private List<float> _zoomFovs = new();
    [SerializeField] private Camera _cam; 
    private int curZoomIndex = 0;

    void Update()
    {
        if(Input.GetButtonUp("ZoomIn")) 
        {
            if(curZoomIndex < _zoomFovs.Count-1) 
            {
                curZoomIndex++; 
                _cam.fieldOfView = _zoomFovs[curZoomIndex];
            }
        }
        if (Input.GetButtonUp("ZoomOut"))
        {
            if (curZoomIndex > 0)
            {
                curZoomIndex--;
                _cam.fieldOfView = _zoomFovs[curZoomIndex];
            }
        }
    }
}
