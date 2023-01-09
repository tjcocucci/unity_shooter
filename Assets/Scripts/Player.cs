using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : DamageableObject
{
    PlayerController controller;
    GunController gunController;
    Camera viewCamera;
    public float moveSpeed = 7f;

    protected override void Start()
    {
        base.Start();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNextWaveStart += ResetPosition;
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
    }

    void ResetPosition(int i) {
        transform.position = Vector3.zero + Vector3.up;
    }

    void Update()
    {
        // Movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 velocity = moveInput.normalized * moveSpeed;
        controller.Move(velocity);

        MapGenerator map = FindObjectOfType<MapGenerator>();

        // Aim input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 intersectionPoint = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, lookPoint);
            controller.LookAt(intersectionPoint);
        }

        // Weapon input
        if (Input.GetMouseButton(0)) {
            gunController.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0)) {
            gunController.OnTriggerRelease();
        }

    }
}
