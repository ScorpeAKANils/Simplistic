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
    [SerializeField, Range(0, byte.MaxValue)] private byte _magSize  = 30;
    [SerializeField] private LineRenderer _lineRenderer; 
    [Networked] public byte AmmoInMag { get; private set; }
    public BasicSpawner Spawner { get; private set; }
    private bool _canFire = true;
    private bool _reload;

    private void Start()
    {
        Spawner = FindObjectOfType<BasicSpawner>();
        AmmoInMag = _magSize; 

    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1")) 
        {
            _fireButtonPressed = true;
        }

        if(Input.GetKeyDown(KeyCode.R)) 
        {
            _reload = true; 
        }
    }
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false | _canFire == false) 
        {
            return;
        }
        if (AmmoInMag < _magSize && _reload)
        {
            _reload = false; 
            AmmoInMag = _magSize;
        }
        if (_fireButtonPressed) 
        {
            _canFire = false; 
            _fireButtonPressed = false;
            RPC_VisualieShot(Spawner.RunnerRef, this); 
            Shoot(GunBarrel.position, GunBarrel.forward);
            StartCoroutine(FireCoolDown(0.1f)); 
        }
    }
    IEnumerator FireCoolDown(float time)
    {
        yield return new WaitForSeconds(time);
        RPC_DisableDebugLine(Spawner.RunnerRef, this); 
        _canFire = true; 
    }
    void Shoot(Vector3 pos, Vector3 dir) 
    {
        AmmoInMag--; 
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
        gunRef.StartCoroutine(FireBullet.TextLifeTime(3f));  
        gunRef.Spawner.ErasePlayer(enemy); 
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public static void RPC_VisualieShot(NetworkRunner runner, FireBullet gunRef, RpcInfo info = default) 
    {
        gunRef._lineRenderer.enabled = true; 
        gunRef._lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        gunRef._lineRenderer.startColor = Color.red;
        gunRef._lineRenderer.endColor = Color.red;
        gunRef._lineRenderer.positionCount = 2;
        Vector3 endPos = gunRef.GunBarrel.position + (gunRef.GunBarrel.forward * 100f); 
        gunRef._lineRenderer.SetPosition(0, gunRef.GunBarrel.position);
        gunRef._lineRenderer.SetPosition(1, endPos);
        gunRef.AmmoInMag--;
        Debug.Log(gunRef.AmmoInMag); 

    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public static void RPC_DisableDebugLine(NetworkRunner runner, FireBullet gunRef, RpcInfo info = default)
    {
        gunRef._lineRenderer.enabled = false; 
    }

    public static IEnumerator TextLifeTime(float duration) 
    {
        yield return new WaitForSeconds(duration); 
        var Text = FindObjectOfType<TextMeshProUGUI>();
        Text.text = string.Empty;
    }
}
