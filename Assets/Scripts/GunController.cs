using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    Gun _currentGun;
    public Transform _weaponHold;
    public Gun[] _allGuns;

    void Start()
    {
        
    }
    public void EquipGun(Gun gunEquipped)
    {
        if(_currentGun != null)
        {
            Destroy(_currentGun.gameObject);
        }
        _currentGun = Instantiate(gunEquipped,_weaponHold.position,_weaponHold.rotation) as Gun;
        _currentGun.transform.parent = _weaponHold;
    }


    public void EquipGun(int weaponIndex)
    {
        EquipGun(_allGuns[weaponIndex]);
    }
    // To fire bullets
    public void OnTriggerHold()
    {
        if(_currentGun != null)
        {
            _currentGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if(_currentGun != null)
        {
            _currentGun.OnTriggerRelease();
        }
    }

    public float GunHeight
    {
        get{
            return _weaponHold.position.y;
        }
    }
    public void Aim(Vector3 aimPoint)
    {
        if (_currentGun != null)
        {
            _currentGun.Aim(aimPoint);
        }
    }

    public void Reload()
    {
        if (_currentGun != null)
        {
            _currentGun.Reload();
        }

    }
}
