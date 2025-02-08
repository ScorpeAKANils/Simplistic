using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRenderTexture : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private Material _mat; 

    // Start is called before the first frame update
    void Start()
    {
        _mat = this.GetComponent<MeshRenderer>().material;
        RenderTexture scopeEffect = new RenderTexture(128, 128, 8);
        scopeEffect.Create();
        _camera.targetTexture = scopeEffect;
        _mat.mainTexture = scopeEffect;
        this.GetComponent<MeshRenderer>().material = _mat; 
    }
}
