using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowResult : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _result; 
    // Start is called before the first frame update
    void Start()
    {
        var DMManager = FindObjectOfType<DeathMatchManager>();
        _result.text = "Winner: " + DMManager.Winner.ToString(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
