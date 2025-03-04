using Fusion;
using TMPro;
using UnityEngine;
using System.Collections;

public class Weapon : NetworkBehaviour
{
    [Networked] public bool CanFire { get; private set; }
    [Networked] public bool IsCollected { get; set; }
    [Networked] public byte AmmoInMag { get; set; }
    [Networked] public bool PlayerFiresGun { get; set; }
    private bool _hitEnemy { get; set; }
    [Networked] private bool _reload { get; set; }
    public float FireDelay { get { return _delayFireTime; } }
    public float ReloadDelay { get { return _reloadDelay; } }
    public float MagSize { get { return _magSize; } }
    public AudioSource Audio;
   
    [SerializeField, Networked] private byte _magSize { get; set; }
    [SerializeField] private Transform GunBarrel; 
    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private WeaponType _type; 
    [SerializeField] private float _delayFireTime = 0.25f;
    [SerializeField] private float _reloadDelay = 1f; 
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _range = 50f;
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] private TextMeshProUGUI _ammoHud;
    [SerializeField] private Health _health;
    [SerializeField] private WeaponRecoil _recoil;
    [SerializeField] private GunSoundManager _soundManager; 
    [SerializeField] private GameObject _killFeed;
    [SerializeField] private GameObject _hitMarker;

    private bool _spawned { get; set; }

    public override void Spawned()
    {
        if (Runner.IsServer) 
        {
            CanFire = true; 
            PlayerFiresGun = false;
        }
        _spawned = true;
        _ammoHud = FindObjectOfType<PlayerHudTag>().GetComponentInChildren<TextMeshProUGUI>();
        _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize;
    }

    public void OnEnable()
    {
        if(_spawned && Runner.IsServer) 
        {
            CanFire = true;
        }
    }

   
    public override void FixedUpdateNetwork()
    {
        if (!_spawned)
            return; 
        if (_ammoHud != null && HasInputAuthority)
            _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize;
        if (_hitEnemy && HasInputAuthority)
        {
            StartCoroutine(ShowHitMarker());
        }
        if (Runner.IsServer)
        {
            _hitEnemy = false;
            if (GetInput(out NetworkInputData data)== false)
                return;
            if(data.Buttons.IsSet(MyButtons.Reload) && AmmoInMag < MagSize && !_reload) 
            {
                Reload(); 
            }
            bool canShoot = data.Buttons.IsSet(MyButtons.Shooting) && CanFire && !_reload && AmmoInMag > 0; 
            if (canShoot)
            {
                PlayerFiresGun = true;
                CanFire = false;
                Shoot(GunBarrel.position, GunBarrel.forward);
                _soundManager.Rpc_PlayShotSound();
                _recoil.ApplyRecoil();
                AmmoInMag--;
                StartCoroutine(FireCoolDown(_delayFireTime));
            }
            else if (data.Buttons.IsSet(MyButtons.Shooting) == false)
            {
                PlayerFiresGun = false;
            }
        }
    }

    public void OnDisable()
    {
        if (!_spawned)
            return; 
        
        _hitMarker.SetActive(false);
        _hitEnemy = false; 
        
        if(_reload) 
        {
            AmmoInMag = 0;
            _reload = false; 
        }
    }

    IEnumerator FireCoolDown(float time)
    {
         yield return new WaitForSeconds(time);
         CanFire = true; 
    }

    void Shoot(Vector3 pos, Vector3 dir) 
    {
        if (!Runner.LagCompensation.Raycast(pos, dir, _range, Runner.LocalPlayer, out LagCompensatedHit hit, ~_ignoreLayer, HitOptions.IncludePhysX))
            return;
        _hitEnemy = SimplisticUttillitys.CheckForHit(hit.Hitbox, _damage, _health.GetPlayer()); 
    }

    IEnumerator ShowHitMarker() 
    {
        _hitMarker.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        _hitMarker.SetActive(false); 
    }

    public void Reload() 
    {
        _reload = true;
        StartCoroutine(ReloadingYield()); 
    }

    IEnumerator ReloadingYield()
    {
        yield return new WaitForSeconds(_reloadDelay);
        AmmoInMag = _magSize; 
        _reload = false; 
    }
}