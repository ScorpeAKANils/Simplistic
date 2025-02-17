using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.SimpleKCC; 
public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] private SimpleKCC _cc;
    [SerializeField] private List<RecoilDirX> _xRecoil = new();
    [SerializeField] private List<RecoilDirY> _yRecoil = new();
    [SerializeField] private float _recoilModifyer;
    [SerializeField] private MouseLook _mouseLook;
    private int _recoilPaternIndex = 0; 
    
    public void ApplyRecoil() 
    {
        float recoilX = (-(float)_xRecoil[_recoilPaternIndex]) * _recoilModifyer;
        float recoilY = (float)_yRecoil[_recoilPaternIndex] * _recoilModifyer;
        Vector2 recoil = new Vector2(recoilX, recoilY);
        _cc.AddLookRotation(recoil, -89, 89);
        _mouseLook.CamTransform.localRotation = Quaternion.Euler(_cc.GetLookRotation().x, 0, 0);
        UpdatePatternIndex();
        Debug.Log("Apply Recoil"); 
    }

    private void UpdatePatternIndex() 
    {
        if (_recoilPaternIndex < _xRecoil.Count - 1 && _recoilPaternIndex < _yRecoil.Count - 1)
            _recoilPaternIndex++; 
        else
            _recoilPaternIndex = 0;
    }
}