using Fusion;
using TMPro;
using UnityEngine;
using System.Collections; 
public class FireBullet : NetworkBehaviour
{
    private bool _fireButtonPressed = false;
    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private Transform GunBarrel; 
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] public LineRenderer LineRend;
    [Networked] private bool _renderLines {  get; set; }
    private NetworkRunner _runnerBulletRef;
    public BasicSpawner Spawner { get; private set; } 
    private void Start()
    {
        _runnerBulletRef = FindObjectOfType<NetworkRunner>();
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
            Shoot(GunBarrel.position, transform.forward); 
        }
        RenderLIne();
    }
    void Shoot(Vector3 pos, Vector3 dir) 
    {
        RPC_SendShotInfo(Spawner.RunnerRef, Spawner.RunnerRef.LocalPlayer, this); 
        if(Physics.Raycast(pos, dir, out RaycastHit hit,  30f, _playerLayer)) 
        {
            PlayerRef enemyPlayer = hit.collider.gameObject.GetComponent<Health>().GetPlayer();
            RPC_SendHitInfo(Spawner.RunnerRef, enemyPlayer, Spawner.RunnerRef.LocalPlayer, this); 
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public static void RPC_SendHitInfo(NetworkRunner runner, PlayerRef enemy, PlayerRef self, FireBullet gunRef,  RpcInfo info = default)
    {
        var Text = FindObjectOfType<TextMeshProUGUI>();
        Text.text = enemy.ToString() + " was killed by: " + self;
        gunRef.Spawner.ErasePlayer(enemy); 
    }

    [Rpc(RpcSources.All, RpcTargets.InputAuthority, InvokeLocal = false,  Channel = RpcChannel.Reliable)]
    public static void RPC_SendShotInfo(NetworkRunner runner, PlayerRef self, FireBullet fb,  RpcInfo info = default)
    {
        if(runner.LocalPlayer == self) 
        {
            fb._renderLines = true;  
        }
    }

    private void RenderLIne() 
    {
        if(_renderLines) 
        {
            _renderLines = false; 
            LineRend.enabled = true;
            LineRend.SetPosition(0, GunBarrel.position);
            LineRend.SetPosition(1, GunBarrel.position + (transform.forward * 30f));
            StartCoroutine(DisableLineAfterDelay(LineRend, 0.5f));
        }
    }
    public IEnumerator DisableLineAfterDelay(LineRenderer lineRenderer, float delay)
    {
        yield return new WaitForSeconds(delay);
        lineRenderer.enabled = false;
    }
}
