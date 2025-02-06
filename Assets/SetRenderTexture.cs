using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRenderTexture : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] RenderTexture _rTex; 
    // Start is called before the first frame update
    void Start()
    {
        //if (_camera != null && _rTex != null)
        //{
        //    _camera.targetTexture = _rTex;
        //}
    }
}
