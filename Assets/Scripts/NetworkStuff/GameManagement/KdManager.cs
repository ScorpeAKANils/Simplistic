using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KdManager : NetworkBehaviour
{

    [SerializeField] private AudioSource KillSound;
    [SerializeField] private TextMeshProUGUI _kdText; 
    [Networked] int _kills { get; set; }
    [Networked] int _deaths { get; set; }
    public override void Spawned () 
    {
        _kdText = FindObjectOfType<KdTagText>().GetComponent<TextMeshProUGUI>();
        _kdText.text = "Kills: 0\nDeaths: 0"; 
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    public void Rpc_AddKill() 
    {
        _kills++; 
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, Channel = RpcChannel.Reliable)]
    public void Rpc_UpdateKDWithSound(int kills, int deaths)
    {
        KillSound.Play(); 
        _kdText.text = "Kills: " + kills.ToString() + "\nDeaths: " + deaths.ToString();
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, Channel = RpcChannel.Reliable)]
    public void Rpc_UpdateKDWithoutSound(int kills, int deaths)
    {
        _kdText.text = "Kills: " + kills.ToString() + "\nDeaths: " + deaths.ToString();
    }
    public int GetKills() { return _kills; }
    public int GetDeaths() { return _deaths; }
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    public void Rpc_AddDeath()
    {
        _deaths++; 
    }
}
