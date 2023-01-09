using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum WeaponType {Automatic, Burst, Single};
    public WeaponType weaponType;

    public Shell shellPrefab;
    public Transform muzzlePosition;
    public Projectile projectile;
    public Transform shellSpawner;

    public int burstSize = 3;
    int shotsRemainingInBurst;

    public float msToShoot = 75f;
    public float projectileSpeed = 35.0f;

    MuzzleFlash muzzleFlash;
    float nextShotTime;
    bool triggerReleasedSinceLastShot;

    void Start() {
        shotsRemainingInBurst = burstSize;
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    public void Shoot() {
        if (nextShotTime < Time.time) {
            if (weaponType == WeaponType.Automatic) {
                // continue
            } else if (weaponType == WeaponType.Single && !triggerReleasedSinceLastShot) {
                return;
            } else if (weaponType == WeaponType.Burst) {
                if (shotsRemainingInBurst == 0) {
                    return;
                }
                shotsRemainingInBurst--;
            }

            nextShotTime = Time.time + msToShoot / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzlePosition.position, muzzlePosition.rotation) as Projectile;
            newProjectile.SetSpeed(projectileSpeed);

            Shell newShell = Instantiate(shellPrefab, shellSpawner.position, shellSpawner.rotation) as Shell;
            muzzleFlash.Activate();
        }
    }

    public void OnTriggerHold() {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease() {
        triggerReleasedSinceLastShot = true;
        if (weaponType == WeaponType.Burst) {
            shotsRemainingInBurst = burstSize;
        }
    }
}
