using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion; 

public class SessionObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_seassionNameText;
    [SerializeField] private TextMeshProUGUI m_seassionPlayerCountText;

    private bool m_initialized = false; 
    private string m_sessionName;
    private int m_playerCount;
    private SessionInfo m_info;
    private GameObject m_spawnerObj;
    private NetworkRunner m_runner; 
    public void Init(SessionInfo info, NetworkRunner runner, GameObject obj) 
    {
        m_sessionName = info.Name;
        m_playerCount = info.PlayerCount;
        m_info = info;
        m_runner = runner; 
        m_spawnerObj = obj;
        m_seassionNameText.text = m_sessionName; 
        m_seassionPlayerCountText.text = m_playerCount.ToString() + "/7";
        m_initialized = true; 
    }
    void Update()
    {
        if (!m_initialized)
            return; 
       if(m_info.PlayerCount != m_playerCount) 
        {
            m_playerCount = m_info.PlayerCount;
            m_seassionPlayerCountText.text = m_playerCount.ToString() + "/7";
        } 
    }

    public void JoinSession() 
    {
        var res = SessionManager.JoinLobby(m_runner, m_sessionName, m_spawnerObj); 
        if(res.IsCompletedSuccessfully) 
        {
            Debug.Log("Yay"); 
        } else 
        {
            Debug.LogError("Couldnt join game"); 
        }
    }
}
