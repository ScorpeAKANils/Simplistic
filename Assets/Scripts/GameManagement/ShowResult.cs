using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowResult : SimulationBehaviour
{
    public GameObject EndResultCam;
    public GameObject EndScreen;
    public GameObject NormalCam;

    public void ToggleEndScreen(bool val) 
    {
        EndResultCam.SetActive(val);
        EndResultCam.GetComponent<Camera>().enabled = val;
        EndResultCam.GetComponent<AudioListener>().enabled = val;
        EndScreen.SetActive(val);
        NormalCam.SetActive(!val); 
        NormalCam.GetComponent<Camera>().enabled = !val;
        NormalCam.GetComponent<AudioListener>().enabled = !val;
    }
}
