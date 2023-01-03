using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class GunController : MonoBehaviour
{

    public Transform weaponHold;
    public Gun startingGun;
    public Gun equippedGun;

    void Start() {
        if (startingGun != null) {
            EquipGun(startingGun);
        }
    }

    void EquipGun(Gun gun) {
        if (equippedGun != null) {
            Destroy(equippedGun);
        }
        equippedGun = Instantiate(startingGun, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
    }

    public void Shoot() {
        if (equippedGun != null) {
            equippedGun.Shoot();
        }
    }
}
