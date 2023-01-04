using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody myRigidbody;
    Vector3 velocity;
    float rotationSpeed = 10;
    Vector3 lookPoint;
    // float smoothMagnitude;
    // float smoothMoveVelocity;
    // float smoothMoveTime = 0.1f;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // smoothMagnitude = Mathf.SmoothDamp(smoothMagnitude, velocity.magnitude, ref smoothMoveVelocity, smoothMoveTime);
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
        myRigidbody.MoveRotation(Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (lookPoint - transform.position), rotationSpeed * Time.deltaTime));
        // myRigidbody.MoveRotation(Quaternion.Slerp() FromToRotation(transform.forward, lookPoint));
    }



    public void Move(Vector3 _velocity) {
        velocity = _velocity;
    }

    public void LookAt(Vector3 point) {
        lookPoint = new Vector3(point.x, transform.position.y, point.z);
        // print("from "+ transform.forward + "   to " + lookPoint);
        // transform.LookAt(lookPoint);
    }

}
