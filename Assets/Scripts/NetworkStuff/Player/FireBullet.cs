using Fusion;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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

    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private WeaponType _type; 
    [SerializeField] private float _delayFireTime = 0.25f;
    [SerializeField] private float _recoilFactor = 0.2f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _range = 50f;
    [SerializeField] private Transform _gunBarrel;
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField, Networked] private byte _magSize { get; set; }
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private TextMeshProUGUI _ammoHud;
    [SerializeField] public AudioSource Audio; 
    [SerializeField] private List<RecoilDirY> _yPattern = new();
    [SerializeField] private List<RecoilDirX> _xPattern = new();
    [SerializeField] private Animator _anim;
    [SerializeField] private bool _isFUllAutomatic = false;
    [SerializeField] private GameObject _hitMarker;
    [SerializeField] private Health _health; 
    private int enemyHitCounter = 0; 
    public Animator Anim { get {  return _anim; } }
    public float FireDelay { get { return _delayFireTime;  } }
    [Networked] public bool IsCollected { get; set; }
    [Networked] public bool PlayerFiresGun { get; set; } 
    private int patternIndex = 0; 
    public Transform GunBarrel { get { return _gunBarrel; } }
    [Networked] public byte AmmoInMag { get; set; }
    
    public BasicSpawner Spawner { get; private set; }
    [SerializeField] private GameObject _killFeed; 
    [Networked] private bool _canFire { get; set; }
    public bool CanFire { get { return _canFire; } }
    private bool _reload;
    private bool _isShooting;
    private bool _spawned = false; 

    private void Start()
    {
        Spawner = FindObjectOfType<BasicSpawner>();
        _hitMarker = GameObject.FindObjectOfType<HitmarkerTag>().gameObject;
        _ammoHud = FindObjectOfType<PlayerHudTag>().GetComponentInChildren<TextMeshProUGUI>();
        _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize;
    }

    public override void Spawned()
    {
        base.Spawned();
        if (Runner.IsServer) 
        {
            _canFire = true; 
            AmmoInMag = 30;
            _magSize = 30;
            PlayerFiresGun = false; 
        }
        _spawned = true; 
    }
    public float GetXRecoile(NetworkInputData data)
    {
        if (_xPattern == null | (_isShooting == false))
            return 0;
        return (float)_xPattern[patternIndex] * _recoilFactor; 
    }

    public float GetYRecoile(NetworkInputData data)
    {
        if (_yPattern == null | _isShooting == false)
            return 0; 
        return (float)_yPattern[patternIndex] * _recoilFactor;
    }
    public void OnEnable()
    {
        if(_spawned && Runner.IsServer) 
        {
            _canFire = true;
        }
        if (_hitMarker == null)
            _hitMarker = GameObject.FindObjectOfType<HitmarkerTag>().gameObject;
    }

    
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer) 
        {
            if(AmmoInMag <= 0) 
            {
                AmmoInMag = _magSize; 
            }
             if (GetInput(out NetworkInputData data) && AmmoInMag > 0)
             {
                 if (data.Buttons.IsSet(MyButtons.Shooting) && _canFire)
                 {
                     PlayerFiresGun = true;
                     _isShooting = true;
                     Shoot(_gunBarrel.position, _gunBarrel.forward);
                     if (!_isFUllAutomatic) 
                     {
                         data.Buttons.Set(MyButtons.Shooting, false);
                     } 
                     AmmoInMag--;
                     StartCoroutine(FireCoolDown(_delayFireTime));
                 }
                 else
                 {
                     _isShooting = false;
                     PlayerFiresGun = false; 
                 }
             }
             else 
             {
                 _isShooting = false;
                 PlayerFiresGun = false;
             }
        }
        if(_ammoHud != null && HasInputAuthority) 
        {
            _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize;
        }
    }

        IEnumerator FireCoolDown(float time)
        {
             _canFire = false; 
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

    void Shoot(Vector3 pos, Vector3 dir) 
    {
        if (Runner.LagCompensation.Raycast(pos, dir, _range, Runner.LocalPlayer, out LagCompensatedHit hit, ~_ignoreLayer, HitOptions.IncludePhysX))
        {
            Health playerHit = null; 
            try 
            {
                playerHit = hit.Hitbox.GetComponent<Health>(); 
            } 
            catch
            {
                return; 
            }
            PlayerRef target = playerHit.GetPlayer();
            playerHit.DealDamage(target, _damage, _health.GetPlayer());
            StartCoroutine(ActivateHitmarker()); 
        }
    }
    public void ResetCanFire() 
    {
        _canFire = true; 
    }


    IEnumerator ActivateHitmarker() 
    {
        _hitMarker.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        _hitMarker.SetActive(false); 
    }
}
