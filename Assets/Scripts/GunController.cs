using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class GunController : MonoBehaviour
{

    public Transform weaponHold;
    public Gun equippedGun;
    public Gun[] allGuns;

    void Start() {
    }

    public void EquipGun(int waveIndex) {
        EquipGun(allGuns[waveIndex]);
    }

    public void EquipGun(Gun gun) {
        if (equippedGun != null) {
            Destroy(equippedGun);
        }
        equippedGun = Instantiate(gun, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
    }

    public void OnTriggerHold() {
        if (equippedGun != null) {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease() {
        if (equippedGun != null) {
            equippedGun.OnTriggerRelease();
        }
    }

    public float GetGunHeight () {
        return equippedGun.transform.position.y;
    }

    public void Reload () {
        equippedGun.Reload();
    }

}
