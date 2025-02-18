using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillFeedManager : MonoBehaviour
{
    public static int KillFeedShown = 0;
    public static List<TextMeshProUGUI> KillFeedMessage = new();
    [SerializeField] private List<TextMeshProUGUI> _killFeedMessage = new();
    bool loadedKillMessage = false; 
    private void Update()
    {
        if(loadedKillMessage == true) 
        {
            return; 
        }
        KillFeedMessage = _killFeedMessage;
        loadedKillMessage = true; 
    }

    public static void ShowKillMessage(PlayerRef killedPlayer, PlayerRef killer) 
    {
       //KillFeedMessage[KillFeedShown].text = killer.ToString() + " killed " + killedPlayer.ToString();
       //FindObjectOfType<KillFeedManager>().StartCoroutine(ShowMessageYield(KillFeedMessage[KillFeedShown], 2f));
       //if(KillFeedShown < KillFeedMessage.Count-1) 
       //{
       //    KillFeedShown++; 
       //} else 
       //{
       //    KillFeedShown = 0; 
       //}
    }  //

    private static IEnumerator ShowMessageYield(TextMeshProUGUI msg, float duration) 
    {
        yield return new WaitForSeconds(duration);
        msg.text = string.Empty; 
    }
}
