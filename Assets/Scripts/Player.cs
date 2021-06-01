using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    [SerializeField] float _moveSpeed = 5;
    [SerializeField] Camera _playerCam;
    PlayerController _controller;
    GunController _gunController;
    public CrossHair _crosshair;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
       
    }

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _gunController = GetComponent<GunController>();
        _playerCam = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }
    void OnNewWave(int waveNumber)
    {
        _health = _startHealth;
        _gunController.EquipGun(waveNumber-1);
    }
    // Update is called once per frame
    void Update()
    {
        // Player Movement
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * _moveSpeed;
        _controller.Move(moveVelocity);
        
        // Player Look Around
        Ray rayOnPlayer = _playerCam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * _gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(rayOnPlayer,out rayDistance))
        {
            Vector3 hitpoint = rayOnPlayer.GetPoint(rayDistance);
            //Debug.DrawLine(rayOnPlayer.origin,hitpoint,Color.red);

            _controller.LookAt(hitpoint);
            _crosshair.transform.position = hitpoint;
            _crosshair.TargetDetection(rayOnPlayer);
            if((new Vector2(hitpoint.x, hitpoint.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                _gunController.Aim(hitpoint);
            }
        }

        // Weapon Input
        if (Input.GetMouseButton(0))
        {
            _gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            _gunController.OnTriggerRelease();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _gunController.Reload();
        }
       if(transform.position.y < -1)
        {
            TakeDamage(_startHealth);
            
        }
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }


}
