using Fusion;
using TMPro;
using UnityEngine;

public class FireBullet : NetworkBehaviour
{
    private bool _fireButtonPressed = false;
    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private Transform _gunBarrel;
    [SerializeField] private Transform _player;
    [SerializeField] private LayerMask _playerLayer;
    private NetworkRunner _runnerBulletRef; 
    private void Start()
    {
        _runnerBulletRef = FindObjectOfType<NetworkRunner>();
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
        if(!HasInputAuthority) 
        {
            return; 
        }
        if(_fireButtonPressed) 
        {
            _fireButtonPressed = false;
            Shoot(_gunBarrel.position, transform.forward); 
            RPC_Fire(_gunBarrel.position);
        }
    }
    void Shoot(Vector3 pos, Vector3 dir) 
    {
        Debug.DrawLine(pos, pos + (dir * 30f));
        if(Physics.Raycast(pos, dir, out RaycastHit hit,  30f, _playerLayer)) 
        {
            PlayerRef player = hit.collider.gameObject.GetComponent<Health>().GetPlayer();
            RPC_SendHitInfo(player);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_SendHitInfo(PlayerRef p, RpcInfo info = default)
    {
        var Text = FindObjectOfType<TextMeshProUGUI>();
        Text.text = p.ToString() + " &Basic Spawner Instace == null: " + (BasicSpawner.Instance==null).ToString();
        Debug.Log("Habe " + p + " getroffen"); 
        BasicSpawner.Instance.ErasePlayer(p); 
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_Fire(Vector3 pos, RpcInfo info = default)
    {
        Runner.Spawn(_bulletPrefab, pos, Quaternion.identity);
    }
}
