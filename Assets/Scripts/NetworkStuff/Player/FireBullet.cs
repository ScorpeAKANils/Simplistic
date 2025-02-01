using Fusion;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum RecoilDirY 
{
    Right_Weak = 1,
    Left_Weak = -1,
    Right_Middle = 2,
    Left_Middle = -2,
    Right_Strong = 3,
    Left_Strong = 3
}
public enum RecoilDirX
{
    Up_Weak = 1,
    Down_Weak = 1, 
    Up_Middle = 2, 
    Down_Middle = 2,
    Up_Strong = 3,
    Down_Strong = 3
}

public class FireBullet : NetworkBehaviour
{
    private bool _fireButtonPressed = false;
    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private Transform _gunBarrel;
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] private byte _magSize = 30;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private TextMeshProUGUI _ammoHud;
    [SerializeField] AudioSource _audio; 
    [SerializeField] private List<RecoilDirY> _yPattern = new();
    [SerializeField] private List<RecoilDirX> _xPattern = new();
    [SerializeField] private Animator _anim; 
    private int patternIndex = 0; 
    public LineRenderer LineRenderer { get { return _lineRenderer; } }
    public TextMeshProUGUI AmmoHud { get { return _ammoHud; } }
    public Transform GunBarrel { get { return _gunBarrel; } }
    public GameObject KillFeed { get { return _killFeed; } }
    [Networked] public byte AmmoInMag { get; set; }
    
    public BasicSpawner Spawner { get; private set; }
    [SerializeField] private GameObject _killFeed; 
    private bool _canFire = true;
    public bool CanFire { get { return _canFire; } }
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

    public float GetXRecoile(NetworkInputData data)
    {
        if (_xPattern == null | data.Buttons.IsSet(MyButtons.Shooting) == false)
            return 0;
        return (float)_xPattern[patternIndex] * 0.2f; 
    }

    public float GetYRecoile(NetworkInputData data)
    {
        if (_yPattern == null | data.Buttons.IsSet(MyButtons.Shooting) == false)
            return 0; 
        return (float)_yPattern[patternIndex] * 0.2f;
    }

    private void Update()
    {
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
        if (GetInput(out NetworkInputData data) && AmmoInMag > 0) 
        {
            if(data.Buttons.IsSet(MyButtons.Shooting)) 
            {
                _audio.Play();
                _anim.SetTrigger("Shoot"); 
                _canFire = false;
                _fireButtonPressed = false;
                StaticRpcHolder.RPC_VisualieShot(Spawner.RunnerRef, this, Spawner.RunnerRef.LocalPlayer);
                Shoot(_gunBarrel.position, _gunBarrel.forward);
                StartCoroutine(FireCoolDown(0.2f));
            }
        }
    }
    IEnumerator FireCoolDown(float time)
    {
        yield return new WaitForSeconds(time);
        StaticRpcHolder.RPC_DisableDebugLine(Spawner.RunnerRef, this); 
        _canFire = true; 
        if(patternIndex < _yPattern.Count-1 && patternIndex < _xPattern.Count - 1) 
        {
            patternIndex++; 
        } else 
        {
            patternIndex = 0;  
        }
    }
    void Shoot(Vector3 pos, Vector3 dir) 
    {
        if(Runner.LagCompensation.Raycast(pos, dir, 100f, this.GetComponent<NetworkObject>().InputAuthority, out LagCompensatedHit hit)) 
        {
            try
            {
                    Debug.Log("Gegner getroffen!");
                    PlayerRef enemyPlayer = hit.Hitbox.GetComponent<Health>().GetPlayer();
                    StaticRpcHolder.RPC_SendHitInfo(Spawner.RunnerRef, enemyPlayer, Spawner.RunnerRef.LocalPlayer, this);
            }
            catch
            {
                Debug.Log("That wasnt a player!"); 
            }
        }
        _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize; 
    }
}
