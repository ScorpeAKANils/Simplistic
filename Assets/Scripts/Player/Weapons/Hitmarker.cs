using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitmarker : MonoBehaviour
{
    float _lifeTime = 0.3f;
    float _time = 0f; 
    public void OnEnable()
    {
        _time = 0; 
    }

    private void Update()
    {
        _time += Time.deltaTime; 
        if(_time >= _lifeTime) 
        {
            this.gameObject.SetActive(false); 
        }
    }
}
