using Fusion;
using TMPro;
using UnityEngine;
using System.Collections; 
public class FireBullet : NetworkBehaviour
{
    private bool _fireButtonPressed = false;
    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private Transform GunBarrel; 
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] public LineRenderer LineRend;
    public BasicSpawner Spawner { get; private set; } 
    private void Start()
    {
        Spawner = FindObjectOfType<BasicSpawner>(); 
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1")) 
        {
            _fireButtonPressed = true;
        }
    }
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false) 
        {
            return;
        }
        if(_fireButtonPressed) 
        {
            _fireButtonPressed = false;
            Shoot(GunBarrel.position, GunBarrel.forward); 
        }
    }
    void Shoot(Vector3 pos, Vector3 dir) 
    {
        if(Physics.Raycast(pos, dir, out RaycastHit hit, 100f, ~_ignoreLayer)) 
        {
            try
            {
                var other = hit.collider.GetComponent<Health>();
                if (other.GetPlayer() != Runner.LocalPlayer) //layer check randomly not working in if statemant, only as a layer in raycast as parameter, but than the raycast ignore all the other colliders. But Players shouldnt be able to shoot threw walls lmao 
                {
                    Debug.Log("Gegner getroffen!");
                    PlayerRef enemyPlayer = hit.collider.GetComponent<Health>().GetPlayer();
                    RPC_SendHitInfo(Spawner.RunnerRef, enemyPlayer, Spawner.RunnerRef.LocalPlayer, this);
                }
            }
            catch
            {
                Debug.Log(hit.collider.gameObject); 
            }
        }
        
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public static void RPC_SendHitInfo(NetworkRunner runner, PlayerRef enemy, PlayerRef self, FireBullet gunRef,  RpcInfo info = default)
    {
        var Text = FindObjectOfType<TextMeshProUGUI>();
        Text.text = enemy.ToString() + " was killed by: " + self;
        gunRef.StartCoroutine(FireBullet.TextLifeTime(1f));  
        gunRef.Spawner.ErasePlayer(enemy); 
    }
    public static IEnumerator TextLifeTime(float duration) 
    {
        yield return new WaitForSeconds(duration); 
        var Text = FindObjectOfType<TextMeshProUGUI>();
        Text.text = string.Empty;
    }
}
