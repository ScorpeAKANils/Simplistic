using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Fusion; 

public class ScoreBoard : SimulationBehaviour
{
    public List<TextMeshProUGUI> ScoreTextes = new();

    private void Awake()
    {
        foreach (TextMeshProUGUI t in ScoreTextes)
        {
            t.text = string.Empty; 
        }
    }
}