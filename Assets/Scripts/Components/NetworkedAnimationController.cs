using UnityEngine;
using Fusion;

[RequireComponent(typeof(Animator))]
public class NetworkedAnimationController : NetworkBehaviour
{
    [SerializeField] private Animator _anim;
    private ParamType _curParam { get; set; }
    private string _paramName { get; set; }
    int _intValue { get; set; }
    float _floatValue { get; set; }
    private bool _boolValue { get; set; }

    public override void Render()
    {
        PlayAnimation(); 
    }

    private void PlayAnimation() 
    {
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
                _anim.SetBool(_paramName, _boolValue);
                break;
            case ParamType.Trigger:
                _anim.SetTrigger(_paramName);
                break;
            default:
                throw new System.Exception("Undefined Parameter type");
        }
        
        _curParam = ParamType.None; 
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_SetTrigger(string paramName) 
    {
        _curParam = ParamType.Trigger; 
        _paramName = paramName;
    }

    public void SetInt(string paramName, int val) 
    {
        _curParam = ParamType.Int;
        _paramName = paramName; 
        _intValue = val;
    }

    public void SetFloat(string paramName, float val) 
    {
        _curParam = ParamType.Float;
        _paramName = paramName;
        _floatValue = val;
    }

    public void SetBool(string paramName, bool val) 
    {
        _curParam = ParamType.Bool;
        _paramName = paramName;
        _boolValue = val;
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