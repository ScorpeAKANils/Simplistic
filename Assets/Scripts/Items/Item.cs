using Fusion;
using UnityEngine;

public abstract class Item : NetworkBehaviour
{
    public bool DestroyWhenUsed; 
    public abstract void OnInteract(PlayerRef player, ItemSpawner i); 
}
