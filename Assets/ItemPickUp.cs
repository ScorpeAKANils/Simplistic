using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private LayerMask _itemLayer;
    [SerializeField]
    private Health _health;

    private List<ItemSpawner> _itemSpawner = new();

    private void Start()
    {
        _itemSpawner = ItemSpawner.FindObjectsByType<ItemSpawner>(FindObjectsSortMode.None).ToList<ItemSpawner>(); 
    }


    private void Update()
    {
        foreach(ItemSpawner i in _itemSpawner) 
        {
            if((i.transform.position - this.transform.position).sqrMagnitude <= 1 && i._spawned == true) 
            {
                i.Item.OnInteract(_health.GetPlayer()); 
            }
        }
    }
}
