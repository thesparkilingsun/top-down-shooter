using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]

public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attackig};
    State _currentState;
    NavMeshAgent _pathFinder;
    Transform _target;
    Material _enemySkinMaterial;
    Color _ogColor;
    float _attackDistanceThreshold = 0.5f;
    float _timeBtwAttack = 1;
    public ParticleSystem _deathEffect;
    float _nextAttackTime;
    float _enemyCollisionRadius;
    float _playerCollisionRadius;
    float _damage = 1;
    // livingEntity
    LivingEntity _targetEntity;

    public static event System.Action _OnDeathStatic;
    

    bool _hasTarget;
     void Awake()
    {
        base.Start();
        _pathFinder = GetComponent<NavMeshAgent>();
        

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            
            _hasTarget = true;
            _target = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
            _targetEntity = _target.GetComponent<LivingEntity>();
           
            _enemyCollisionRadius = GetComponent<CapsuleCollider>().radius;
            _playerCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;

            
        }
    }
    protected override void Start()
    {
        base.Start();
       
        _enemySkinMaterial = GetComponent<Renderer>().material;
        _ogColor = _enemySkinMaterial.color;

        if (_hasTarget) 
        {
            _currentState = State.Chasing;
            _targetEntity.OnDeath += OnTargetDeath;
            StartCoroutine(UpdatePath());
        }
    }

   public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer,float enemyHealth, Color skinColor)
    {
        _pathFinder.speed = moveSpeed;
        if (_hasTarget)
        {
            _damage = Mathf.Ceil(_targetEntity._startHealth / hitsToKillPlayer);
            _startHealth = enemyHealth;
            
            _deathEffect.startColor= new Color(skinColor.r, skinColor.g, skinColor.b, 1);
            _enemySkinMaterial = GetComponent<Renderer>().material;
            _enemySkinMaterial.color = skinColor;
            _ogColor = _enemySkinMaterial.color;

        }


    }
    public override void TakeHit(float damage, Vector3 hitPoint,Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if(damage >= _health)
        {
            if(_OnDeathStatic != null)
            {
                _OnDeathStatic();
            }
            Destroy(Instantiate(_deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, _deathEffect.startLifetime);
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
        }
        base.TakeHit(damage, hitPoint,hitDirection);
    }


    void OnTargetDeath()
    {
        _hasTarget = false;
        _currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (_hasTarget) { 
            if(Time.time > _nextAttackTime) { 
            float sqrDistToTarget = (_target.position - transform.position).sqrMagnitude;
            if(sqrDistToTarget < Mathf.Pow(_attackDistanceThreshold + _enemyCollisionRadius + _playerCollisionRadius, 2))
                {
                    _nextAttackTime = Time.time + _timeBtwAttack;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
    }



    IEnumerator Attack()
    {
        _currentState = State.Attackig;
        _pathFinder.enabled = false;
        Vector3 originalPos = transform.position;
        Vector3 dirToTarget = (_target.position - transform.position).normalized;
        Vector3 attackPos = _target.position - dirToTarget * (_playerCollisionRadius);

        _enemySkinMaterial.color = Color.red;
        float attackSpeed = 3;
        float percent = 0;
        bool hasAppliedDamage = false;

        while(percent <= 1)
        {
            if(percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                _targetEntity.TakeDamage(_damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);
            yield return null;
        }

        _enemySkinMaterial.color = _ogColor; 
        _currentState = State.Chasing;
        _pathFinder.enabled = true;
    }
    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;
        while(_hasTarget)
        {
            if(_currentState == State.Chasing) {
                Vector3 dirToTarget = (_target.position - transform.position).normalized;
                Vector3 targetPosition = _target.position - dirToTarget * (_enemyCollisionRadius + _playerCollisionRadius + _attackDistanceThreshold/2 )  ;
                if (_dead == false)
                {
                    _pathFinder.SetDestination(targetPosition);
                
                }
                }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
