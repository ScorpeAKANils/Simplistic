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
    public Animator Anim { get; private set; }
    public AudioSource Audio; 

    [SerializeField, Networked] private byte _magSize { get; set; }
    [SerializeField] private Transform GunBarrel; 
    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private WeaponType _type; 
    [SerializeField] private float _delayFireTime = 0.25f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _range = 50f;
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private TextMeshProUGUI _ammoHud;
    [SerializeField] private bool _isFUllAutomatic = false;
    [SerializeField] private Health _health; 
    
    [SerializeField] private GameObject _killFeed; 
    private bool _isShooting;
    private bool _spawned = false; 

    private void Start()
    {
        //_hitMarker = GameObject.FindObjectOfType<HitmarkerTag>().gameObject;
        _ammoHud = FindObjectOfType<PlayerHudTag>().GetComponentInChildren<TextMeshProUGUI>();
        _ammoHud.text = "Ammo: " + AmmoInMag.ToString() + "/" + _magSize;
    }

    public override void Spawned()
    {
        base.Spawned();
        if (Runner.IsServer) 
        {
            CanFire = true; 
            AmmoInMag = 30;
            _magSize = 30;
            PlayerFiresGun = false; 
        }
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
        if (Runner.IsServer) 
        {
            if(AmmoInMag <= 0) 
            {
                AmmoInMag = _magSize; 
            }
             if (GetInput(out NetworkInputData data) && AmmoInMag > 0)
             {
                if (data.Buttons.IsSet(MyButtons.Shooting) && CanFire)
                {
                    PlayerFiresGun = true;
                    _isShooting = true;
                    Shoot(GunBarrel.position, GunBarrel.forward);
                    CanFire = false;
                    if (!_isFUllAutomatic)
                    {
                        data.Buttons.Set(MyButtons.Shooting, false);
                    }
                    AmmoInMag--;
                    StartCoroutine(FireCoolDown(_delayFireTime));
                }
                else if (data.Buttons.IsSet(MyButtons.Shooting) == false) 
                {
                    PlayerFiresGun = false;
                    _isShooting = false; 
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
             yield return new WaitForSeconds(time);
             CanFire = true; 
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
        }
    }
    public void ResetCanFire() 
    {
        CanFire = true; 
    }
}