using Fusion;
using TMPro;
using UnityEngine;
using System.Collections;
public class FireBullet : NetworkBehaviour
{
    private bool _fireButtonPressed = false;
    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private Transform _gunBarrel;
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] private byte _magSize = 30;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private TextMeshProUGUI _ammoHud;
    public LineRenderer LineRenderer { get { return _lineRenderer; } }
    public TextMeshProUGUI AmmoHud { get { return _ammoHud; } }
    public Transform GunBarrel { get { return _gunBarrel; } }
    public GameObject KillFeed { get { return _killFeed; } }
    [Networked] public byte AmmoInMag { get; set; }

    public BasicSpawner Spawner { get; private set; }
    [SerializeField] private GameObject _killFeed; 
    private bool _canFire = true;
    private bool _reload;

    private void Start()
    {
        Spawner = FindObjectOfType<BasicSpawner>();
        AmmoInMag = _magSize;
        _ammoHud = FindObjectOfType<PlayerHudTag>().GetComponentInChildren<TextMeshProUGUI>();
        _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize;
        _lineRenderer.enabled = false;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.positionCount = 2;
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
        if (AmmoInMag < _magSize && _reload | AmmoInMag <= 0)
        {
            _reload = false; 
            AmmoInMag = _magSize;
            if (_ammoHud != null) 
            {
                _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize; 
            }
        }
        if (_fireButtonPressed && AmmoInMag > 0) 
        {
            _canFire = false; 
            _fireButtonPressed = false;
            StaticRpcHolder.RPC_VisualieShot(Spawner.RunnerRef, this, Spawner.RunnerRef.LocalPlayer); 
            Shoot(_gunBarrel.position, _gunBarrel.forward);
            StartCoroutine(FireCoolDown(0.1f)); 
        }
    }
    IEnumerator FireCoolDown(float time)
    {
        yield return new WaitForSeconds(time);
        StaticRpcHolder.RPC_DisableDebugLine(Spawner.RunnerRef, this); 
        _canFire = true; 
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
                    StaticRpcHolder.RPC_SendHitInfo(Spawner.RunnerRef, enemyPlayer, Spawner.RunnerRef.LocalPlayer, this);
                }
            }
            catch
            {
                Debug.Log(hit.collider.gameObject); 
            }
        }
        _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize; 
    }
}
