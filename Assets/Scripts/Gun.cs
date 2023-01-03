using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzlePosition;
    public Projectile projectile;
    public float msToShoot = 75f;
    public float projectileSpeed = 35.0f;

    float nextShotTime;

    void Start() {
    }

    public void Shoot() {
        if (nextShotTime < Time.time) {
            nextShotTime = Time.time + msToShoot / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzlePosition.position, muzzlePosition.rotation) as Projectile;
            newProjectile.SetSpeed(projectileSpeed);
        }
    }
}
