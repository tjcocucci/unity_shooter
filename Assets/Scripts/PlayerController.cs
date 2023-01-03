using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Rigidbody myRigidbody;
    Vector3 velocity;
    Vector3 lookPoint;
    // float smoothMagnitude;
    // float smoothMoveVelocity;
    // public float smoothMoveTime = 0.1f;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // smoothMagnitude = Mathf.SmoothDamp(smoothMagnitude, velocity.magnitude, ref smoothMoveVelocity, smoothMoveTime);
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
        transform.LookAt(lookPoint);
    }

    public void Move(Vector3 _velocity) {
        velocity = _velocity;
    }

    public void LookAt(Vector3 point) {
        lookPoint = new Vector3(point.x, transform.position.y, point.z);
    }

}
