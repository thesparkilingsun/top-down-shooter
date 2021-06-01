using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FIREMODE { Auto, Burst, Single};
    public FIREMODE _fireMode;
    public Transform[] _projectileSpawn;
    public Projectile _projectile;
    public float _timeBetweenShots = 100;
    public float _bulletVelocity = 35;
    float _shotTime;
    public int _burstCount;
    public int _projectilePerMag;
    public float _reloadTime = .3f;

    [Header("Effects")]
    public Transform _shell;
    public Transform _shellEject;

    MuzzelFlash _muzzelFlash;
    bool _triggerReleasedLS;
    int _remainingBusrtShots;
    int _remainingPrjMag;
    bool isReloading;
    public AudioClip _shootAudio;
    public AudioClip _reloadAudio;

    [Header("Recoil")]
    public Vector2 _kickMinMax = new Vector2(.05f,.2f);
    public Vector2 _recoilMinMax = new Vector2(3,5);
    public float _recoilMov = .1f;
    public float _recoilRotation = .1f;


    float _nextShotTime;
    Vector3 _recoilSmoothVel;
    float _recoilAngle;
    float _recoilSmoothDamVel;

    void Start()
    {
        _muzzelFlash = GetComponent<MuzzelFlash>();
        _remainingBusrtShots = _burstCount;
        _remainingPrjMag = _projectilePerMag;
    }

     void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref _recoilSmoothVel, _recoilMov);
        _recoilAngle = Mathf.SmoothDamp(_recoilAngle, 0, ref _recoilSmoothDamVel, _recoilRotation);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * _recoilAngle;

        if(!isReloading && _remainingPrjMag == 0)
        {
            Reload();
        }
    }

    void Shoot()
    {
        if(!isReloading && Time.time > _shotTime && _remainingPrjMag > 0) {
                if (_fireMode == FIREMODE.Burst)
                    { if(_remainingBusrtShots == 0)
                        {
                            return;
                        }
                        _remainingBusrtShots --;
                }
                else if(_fireMode == FIREMODE.Single)
                {
                        if (!_triggerReleasedLS)
                        {
                            return;
                        }
                }
            for (int i = 0; i < _projectileSpawn.Length; i++)
            {
                if (_remainingPrjMag == 0) {
                    break;
                }
                    
                _remainingPrjMag--;
                _shotTime = Time.time + _timeBetweenShots / 1000;
                Projectile newProjectile = Instantiate(_projectile, _projectileSpawn[i].position, _projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(_bulletVelocity);
                
            }
            Instantiate(_shell, _shellEject.position, _shellEject.rotation);
            _muzzelFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(_kickMinMax.x,_kickMinMax.y);
            _recoilAngle += Random.Range(_recoilMinMax.x, _recoilMinMax.y);
            _recoilAngle = Mathf.Clamp(_recoilAngle,0,30);
            AudioManager.instance.PlaySound(_shootAudio,transform.position);
        }
        
    }
    public void Reload()
    {
        if(!isReloading && _remainingPrjMag != _projectilePerMag)
        {
            StartCoroutine(AnimateReload());
            AudioManager.instance.PlaySound(_reloadAudio, transform.position);
        }
        
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / _reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while(percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;
            yield return null;
        }
        isReloading = false;
        _remainingPrjMag = _projectilePerMag;
    }
    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
        }
        
    }
    public void OnTriggerHold()
    {
        Shoot();
        _triggerReleasedLS = false;
    }

    public void OnTriggerRelease()
    {
        _triggerReleasedLS = true;
        _remainingBusrtShots = _burstCount;
    }


}
