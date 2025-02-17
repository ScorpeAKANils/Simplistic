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
    public float FireDelay { get { return _delayFireTime;  } }
    public Animator Anim; 
    public AudioSource Audio; 

    [SerializeField, Networked] private byte _magSize { get; set; }
    [SerializeField] private Transform GunBarrel; 
    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private WeaponType _type; 
    [SerializeField] private float _delayFireTime = 0.25f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _range = 50f;
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] private TextMeshProUGUI _ammoHud;
    [SerializeField] private Health _health;
    [SerializeField] private WeaponRecoil _recoil;
    [SerializeField] private GunSoundManager _soundManager; 
    [SerializeField] private GameObject _killFeed;
    [SerializeField] private GameObject _hitMarker; 
    private bool _spawned = false; 

    public override void Spawned()
    {
        if (Runner.IsServer) 
        {
            CanFire = true; 
            AmmoInMag = 30;
            _magSize = 30;
            PlayerFiresGun = false; 
        }
        _ammoHud = FindObjectOfType<PlayerHudTag>().GetComponentInChildren<TextMeshProUGUI>();
        _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize;
        _spawned = true; 
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
        if (_ammoHud != null && HasInputAuthority)
            _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize;

        if (!Runner.IsServer)
            return; 

        if(AmmoInMag <= 0) 
            AmmoInMag = _magSize; 

        if (GetInput(out NetworkInputData data) == false)
            return; 

        if (data.Buttons.IsSet(MyButtons.Shooting) && CanFire)
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

    IEnumerator FireCoolDown(float time)
    {
         yield return new WaitForSeconds(time);
         CanFire = true; 
    }

    void Shoot(Vector3 pos, Vector3 dir) 
    {
        if (!Runner.LagCompensation.Raycast(pos, dir, _range, Runner.LocalPlayer, out LagCompensatedHit hit, ~_ignoreLayer, HitOptions.IncludePhysX))
            return;
        try 
        {
            Health playerHit = hit.Hitbox.GetComponent<Health>(); 
            PlayerRef target = playerHit.GetPlayer();
            playerHit.DealDamage(target, _damage, _health.GetPlayer());
            _hitMarker.SetActive(true);
        }
        catch { }    
    }
}