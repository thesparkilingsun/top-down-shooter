using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LivingEntity : MonoBehaviour, IDamagable
{
    public float _startHealth;
    public float _health { get; protected set; }
    protected bool _dead;

    public event System.Action OnDeath;
    protected virtual void Start()
    {
        _health = _startHealth;
    }
    public virtual void TakeHit(float damage, Vector3 hitPoint,Vector3 hitDirection)
    {
        //Add more functionality later
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0 && !_dead)
        {
            Die();
        }
    }
    [ContextMenu("SelfDestruct")]
    public virtual void Die()
    {
        _dead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
 
    
}
