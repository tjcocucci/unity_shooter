using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum WeaponType {Automatic, Burst, Single};
    public WeaponType weaponType;

    public Shell shellPrefab;
    public Transform muzzle;
    Transform[] muzzlePositions;
    public Projectile projectile;
    public Transform shellSpawner;

    public int burstSize = 3;
    int shotsRemainingInBurst;

    public float msToShoot = 75f;
    public float projectileSpeed = 35.0f;

    [Header("Recoil")]
    Vector3 recoilVelocity;
    float recoilAngleVelocity;
    public Vector2 recoilDistanceMinmax = new Vector2(.1f, .5f);
    public Vector2 recoilAngleMinmax = new Vector2(2, 5);
    public float recoilMaxAngle = 30;
    public float recoilDuration = 0.15f;
    float recoilAngle;

    [Header("Reload")]
    public float reloadTime = 0.5f;
    public float maxReloadAngle = 50;
    public int maxMagazineSize = 10;
    int bulletsRemainingInMagazine;
    bool isReloading;

    MuzzleFlash muzzleFlash;
    float nextShotTime;
    bool triggerReleasedSinceLastShot;

    void Start() {
        isReloading = false;
        shotsRemainingInBurst = burstSize;
        bulletsRemainingInMagazine = maxMagazineSize;
        muzzleFlash = GetComponent<MuzzleFlash>();
        muzzlePositions = muzzle.GetComponentsInChildren<Transform>();
    }

    void LateUpdate() {
        if (!isReloading) {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilVelocity, recoilDuration);
            recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilAngleVelocity, recoilDuration);
            transform.localEulerAngles = Vector3.left * recoilAngle;
        }
    }

    public void Shoot() {
        if (nextShotTime < Time.time && !isReloading && bulletsRemainingInMagazine > 0) {
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

            for (int i=0; i<muzzlePositions.Length; i++) {
                bulletsRemainingInMagazine--;
                Projectile newProjectile = Instantiate(projectile, muzzlePositions[i].position, muzzlePositions[i].rotation) as Projectile;
                newProjectile.SetSpeed(projectileSpeed);
                Shell newShell = Instantiate(shellPrefab, shellSpawner.position, shellSpawner.rotation) as Shell;
            }
            nextShotTime = Time.time + msToShoot / 1000;

            muzzleFlash.Activate();

            transform.localPosition = transform.localPosition + Vector3.back * Random.Range(recoilDistanceMinmax.x, recoilDistanceMinmax.y);
            recoilAngle += Random.Range(recoilAngleMinmax.x, recoilAngleMinmax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, recoilMaxAngle);
        }


    }

    public void OnTriggerHold() {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void Reload() {
        if (!isReloading && bulletsRemainingInMagazine < maxMagazineSize) {
            StartCoroutine(ReloadAnimation());
        }
    }

    IEnumerator ReloadAnimation() {
        isReloading = true;

        yield return new WaitForSeconds(.2f);
        float percent = 0;
        float reloadSpeed = 1 / reloadTime;
        Vector3 originalEulerAngles = transform.localEulerAngles;

        while(percent <= 1) {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = Vector3.left * reloadAngle;
            yield return null;
        }

        isReloading = false;
        bulletsRemainingInMagazine = maxMagazineSize;
    }

    public void OnTriggerRelease() {
        triggerReleasedSinceLastShot = true;
        if (weaponType == WeaponType.Burst) {
            shotsRemainingInBurst = burstSize;
        }
    }
}
