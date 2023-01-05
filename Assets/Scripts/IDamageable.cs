using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {

    public void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection);
    public void TakeDamage(float damage);
}
