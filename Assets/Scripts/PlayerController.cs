using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody _rigidbody;
    Vector3 velocity;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

   void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + velocity * Time.fixedDeltaTime);
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 turnPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(turnPoint);
    }
}
