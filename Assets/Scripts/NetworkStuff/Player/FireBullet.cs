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

public enum WeaponType 
{
    Protogun = 0, 
    Sniper = 1
} 
public class FireBullet : NetworkBehaviour
{
    private bool _fireButtonPressed = false;
    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private WeaponType _type; 
    [SerializeField] private float _delayFireTime = 0.25f;
    [SerializeField] private float _recoilFactor = 0.2f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _range = 50f;
    [SerializeField] private Transform _gunBarrel;
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] private byte _magSize = 30;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private TextMeshProUGUI _ammoHud;
    [SerializeField] public AudioSource Audio; 
    [SerializeField] private List<RecoilDirY> _yPattern = new();
    [SerializeField] private List<RecoilDirX> _xPattern = new();
    [SerializeField] private Animator _anim;
    [Networked] public bool IsCollected { get; set; }
    [SerializeField] private bool SetIsCollectedTrueAtStartUp = false;
    private bool _weaponInUse = false;
    private int patternIndex = 0; 
    public LineRenderer LineRenderer { get { return _lineRenderer; } }
    public TextMeshProUGUI AmmoHud { get { return _ammoHud; } }
    public Transform GunBarrel { get { return _gunBarrel; } }
    public GameObject KillFeed { get { return _killFeed; } }
    public byte AmmoInMag { get; set; }
    
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
    }

    public float GetXRecoile(NetworkInputData data)
    {
        if (_xPattern == null | data.Buttons.IsSet(MyButtons.Shooting) == false)
            return 0;
        return (float)_xPattern[patternIndex] * _recoilFactor; 
    }

    public float GetYRecoile(NetworkInputData data)
    {
        if (_yPattern == null | data.Buttons.IsSet(MyButtons.Shooting) == false)
            return 0; 
        return (float)_yPattern[patternIndex] * _recoilFactor;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            _reload = true; 
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
    }

    
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false | _canFire == false) 
        {
            return;
        }
        if (GetInput(out NetworkInputData data) && AmmoInMag > 0)
        {
            if (data.Buttons.IsSet(MyButtons.Shooting))
            {
                _canFire = false;
                _anim.SetTrigger("Shoot"); 
                if (Runner.IsServer)
                {
                    Shoot(_gunBarrel.position, _gunBarrel.forward);
                }
                else
                {
                    RPC_RequestShot(_gunBarrel.position, _gunBarrel.forward, Runner.LocalPlayer);
                }
                AmmoInMag--;
                _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize;
                StartCoroutine(FireCoolDown(_delayFireTime));
            }
        }
        }

        IEnumerator FireCoolDown(float time)
        {
              yield return new WaitForSeconds(time);
              _canFire = true; 
              if(patternIndex < _yPattern.Count-1 && patternIndex < _xPattern.Count - 1) 
              {
                  patternIndex++; 
              } else 
              {
                  patternIndex = 0;  
              }
        }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable, TickAligned = true)]
    public void RPC_RequestShot(Vector3 pos, Vector3 dir, PlayerRef shooter)
    {
        RPC_VisualieShot(this, shooter);
        if (Runner.LagCompensation.Raycast(pos, dir, _range, shooter, out LagCompensatedHit hit))
        {
            try
            {
                PlayerRef enemyPlayer = hit.Hitbox.GetComponent<Health>().GetPlayer();
                ApplyDamage(enemyPlayer, shooter, _damage);
            }
            catch
            {

            }
        }
    }

    public void ApplyDamage(PlayerRef enemyPlayer, PlayerRef attacker, float damage)
    {
        var spawner = FindObjectOfType<BasicSpawner>();
        spawner.RPC_ApplyDamage(enemyPlayer, damage, attacker); 
    }


    void Shoot(Vector3 pos, Vector3 dir) 
    {
        Audio.Play();
        RPC_VisualieShot(this, Runner.LocalPlayer);
        if (Runner.LagCompensation.Raycast(pos, dir, _range, Runner.LocalPlayer, out LagCompensatedHit hit))
        { 
            try
            {
                Debug.Log("Gegner getroffen!");
                PlayerRef enemyPlayer = hit.Hitbox.GetComponent<Health>().GetPlayer();
                ApplyDamage(enemyPlayer, Runner.LocalPlayer, _damage); 
            }
            catch
            {
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void RPC_VisualieShot(FireBullet gunRef, PlayerRef p)
    {
        gunRef.Audio.Play();
    }
}
