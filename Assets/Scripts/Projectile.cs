using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float _speed = 5;
    public LayerMask _collissionMask;
    float _damage = 2;
    float _lifeTime = 1;
    float _bulletLength = .1f;
    public Color _trailColor;
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    void Start()
    {
        Destroy(gameObject, _lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, _collissionMask);
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0],transform.position);
        }
        GetComponent<TrailRenderer>().material.SetColor("_TintColor", _trailColor);
    }
    void FixedUpdate()
    {
        // Calculate projectile Distance from enemy
        float movement = _speed * Time.deltaTime;
        CheckCollissions(movement);
        transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    }

    void CheckCollissions(float movement)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, movement + _bulletLength, _collissionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider,hit.point);
        }
    }
    // Where the projectile hit
    //void OnHitObject(RaycastHit hit)
    //{
    //    //print(hit.collider.gameObject.name + "Dead");

    //    IDamagable damagedObject = hit.collider.GetComponent<IDamagable>();
    //    if(damagedObject != null) {
    //        damagedObject.TakeHit(_damage, hit);
    //    }
    //    GameObject.Destroy(gameObject);

    //}

   void OnHitObject(Collider c, Vector3 hitPoint)
    {

        IDamagable damagedObject = c.GetComponent<IDamagable>();
        if (damagedObject != null)
        {
            damagedObject.TakeHit(_damage,hitPoint,transform.forward);
        }
        GameObject.Destroy(gameObject);
    }


}
