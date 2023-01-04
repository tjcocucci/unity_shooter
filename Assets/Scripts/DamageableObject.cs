using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour, IDamageable
{
    public float startingHealth;
    public float health;
    public bool dead;

    public event System.Action ObjectDied;

    // Start is called before the first frame update
    protected virtual void Start() {
        health = startingHealth;
    }

    public virtual void TakeDamage (float damage, RaycastHit hit) {
        TakeDamage(damage);
    }

    public virtual void TakeDamage (float damage) {
        health -= damage;
        if (health <= 0 && !dead) {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    protected virtual void Die() {
        dead = true;
        GameObject.Destroy(gameObject);
        if (ObjectDied != null) {
            ObjectDied();
        }
    }

}
