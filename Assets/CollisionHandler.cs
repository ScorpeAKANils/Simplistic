using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private LayerMask _itemLayer;
    [SerializeField]
    private Health _health;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == _itemLayer) 
        {
            other.GetComponent<Item>().OnInteract(_health.GetPlayer()); 
        }
    }
}
