using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class LayerManager : NetworkBehaviour
{
    [SerializeField] private LayerMask _layerSelf;
    [SerializeField] private LayerMask _layerOther;
    [SerializeField] private Health _health;

    private bool _allreadyChecked = false;

    private void Update()
    {
        if (_allreadyChecked == true)
        {
            return; 
        }
        if (Runner.LocalPlayer == _health.GetPlayer())
        {
            this.gameObject.layer = _layerSelf;
            _allreadyChecked = true; 
        }
        else
        {
            this.gameObject.layer = _layerOther;
            _allreadyChecked = true; 
        }
    }
}