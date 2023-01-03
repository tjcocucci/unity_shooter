using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float damage = 1.0f;
    public float offset = 0.1f;
    public LayerMask enemyCollisionMask;
    public LayerMask obstacleCollisionMask;

    // Start is called before the first frame update
    void Start() {
        Destroy(gameObject, 3f);
        Collider [] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, enemyCollisionMask);
        if (initialCollisions.Length > 0) {
            IDamageable damageableObject = initialCollisions[0].GetComponent<IDamageable>();
            if (damageableObject != null) {
                damageableObject.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        initialCollisions = Physics.OverlapBox(transform.position, Vector3.one * 0.5f, Quaternion.identity, obstacleCollisionMask);
        if (initialCollisions.Length > 0) {
            Destroy(gameObject);
        }
    }

    void Update() {
        float distance = speed * Time.deltaTime;
        CheckCollisions(distance);
        transform.Translate(Vector3.forward * distance);
    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    void CheckCollisions(float distance) {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance + offset, enemyCollisionMask, QueryTriggerInteraction.Collide)) {
            IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
            if (damageableObject != null) {
                damageableObject.TakeDamage(damage, hit);
            }
            Destroy(gameObject);
        }
        if (Physics.Raycast(ray, out hit, distance, obstacleCollisionMask, QueryTriggerInteraction.Collide)) {
            Destroy(gameObject);
        }
    }

}
