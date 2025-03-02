using UnityEngine;
using Fusion;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(Animator))]
public class NetworkedAnimationController : NetworkBehaviour
{
    [SerializeField] private Animator _anim;
    [Networked] private ParamType _curParam { get; set; }
    [Networked] private string _paramName { get; set; }
    int _intValue { get; set; }
    float _floatValue { get; set; }
    [Networked]private bool _boolValue { get; set; }
    [Networked] private bool _newParamIsOfSameType { get; set; }
    [Networked] private string _oldParam {  get; set; }
    public override void Render()
    {
        PlayAnimation(); 
    }

    private void PlayAnimation() 
    {
        Debug.Log(_curParam);
        switch (_curParam)
        {
            case ParamType.None:
                break;
            case ParamType.Int:
                _anim.SetInteger(_paramName, _intValue);
                break;
            case ParamType.Float:
                _anim.SetFloat(_paramName, _floatValue);
                break;
            case ParamType.Bool:
                if (_newParamIsOfSameType) 
                {
                    ResetBool(); 
                }
                _anim.SetBool(_paramName, _boolValue);
                break;
            case ParamType.Trigger:
                _anim.SetTrigger(_paramName);
                break;
            default:
                throw new System.Exception("Undefined Parameter type");
        }
    }


    public void ResetAnimationState() 
    {
        _curParam = ParamType.None; 
    }
    public void SetTrigger(string paramName) 
    {
        if(_paramName == paramName) 
        {
            return; 
        }
        _curParam = ParamType.Trigger; 
        _paramName = paramName;
    }

    public void SetInt(string paramName, int val) 
    {
        if (paramName == _paramName && val == _intValue)
        {
            return;
        }
        _curParam = ParamType.Int;
        _paramName = paramName; 
        _intValue = val;
    }
    private void ResetBool() 
    {
        if (Runner.IsServer)
            _newParamIsOfSameType = false;
        _anim.SetBool(_oldParam, false);
    }
    public void SetFloat(string paramName, float val) 
    {
        if (paramName == _paramName && val == _floatValue)
        {
            return;
        }
        _curParam = ParamType.Float;
        _paramName = paramName;
        _floatValue = val;
    }

    public void SetBool(string paramName, bool val) 
    {
        if(paramName == _paramName && val == _boolValue) 
        {
            Debug.Log("couldnt set bool: " + paramName +"\n current paramName is: " + _paramName +"\n" + "the values are the same: " + (val == _boolValue)); 
            return; 
        } 
        if(_curParam == ParamType.Bool) 
        {
            _oldParam = _paramName; 
            _newParamIsOfSameType = true;
        }
        _curParam = ParamType.Bool;
        _paramName = paramName;
        _boolValue = val;
        Debug.Log("Set bool " + paramName + " " + val); 
    }

    private enum ParamType 
    {
        None, 
        Int, 
        Float, 
        Trigger, 
        Bool
    }
}