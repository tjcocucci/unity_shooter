using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : DamageableObject
{
    public enum State {Idle, Chasing, Attacking};

    public ParticleSystem DeathEffectPrefab;

    NavMeshAgent pathfinder;
    Transform targetTransform;
    State currentState;
    
    GameObject target;
    DamageableObject targetDamageable;
    public float attackDistance = 2f;
    public float attackDuration = 0.25f;
    public float followDelay = 0.5f;
    public float timeBetweenAttacks = 2f;
    public float damage = 1;
    public Color originalColor;
    public Color attackColor;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;
    bool hasTarget;

    void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        GetComponent<Renderer>().sharedMaterial.color = originalColor;

        target = GameObject.FindGameObjectWithTag("Player"); 
        if (target != null) {
            hasTarget = true;
            targetTransform = target.transform;
            targetDamageable = target.GetComponent<DamageableObject>();
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start()
    {
        base.Start();
        currentState = State.Idle;

        myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        nextAttackTime = Time.time;
        if (hasTarget) {
            currentState = State.Chasing;
            targetDamageable.ObjectDied += OnTargetDeath;
        }

        StartCoroutine(FindPath());
    }

    void Update() {

        if (hasTarget) {
            if (Time.time > nextAttackTime) {
                float sqrDistanceToTarget = Vector3.SqrMagnitude(transform.position - targetTransform.position);
                if (sqrDistanceToTarget <= Mathf.Pow(attackDistance + myCollisionRadius, 2) && Time.time > nextAttackTime) {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    public void SetCharacteristics (Color skinColor, float enemySpeed, int hitsToKillPlayer, float health) {
        startingHealth = health;
        pathfinder.speed = enemySpeed;
        if (targetDamageable != null) {
            damage = targetDamageable.startingHealth / hitsToKillPlayer;
        }
        GetComponent<Renderer>().sharedMaterial.color = skinColor;
    }

    IEnumerator Attack() {
        currentState = State.Attacking;
        GetComponent<Renderer>().material.color = attackColor;


        Vector3 originalPosition = transform.position;
        Vector3 attackPosition = targetTransform.position;

        float attackSpeed = 3;
        float percent = 0;
        bool hasAppliedDamage = false;
        Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
        attackPosition = attackPosition - directionToTarget * myCollisionRadius;

        pathfinder.enabled = false;

        while(percent <= 1) {
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            if (percent >= 0.5f && !hasAppliedDamage) {
                hasAppliedDamage = true;
                targetDamageable.TakeDamage(damage);
                if (targetDamageable.dead) {
                    hasTarget = false;
                }
            }
            yield return null;

        }

        pathfinder.enabled = true;
        GetComponent<Renderer>().material.color = originalColor;
        currentState = State.Chasing;

    }

    void OnTargetDeath() {
        hasTarget = false;
        currentState = State.Idle;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
        if (damage >= health) {
            Destroy(
                Instantiate(
                    DeathEffectPrefab.gameObject,
                    hitPoint,
                    Quaternion.FromToRotation(Vector3.forward, hitDirection)
                ),
            DeathEffectPrefab.main.duration + DeathEffectPrefab.main.startLifetime.constantMax);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    IEnumerator FindPath() {
        while(hasTarget) {
            if (!dead) {
                if (currentState == State.Chasing) {
                    Vector3 targetPosition = new Vector3(targetTransform.position.x, 0, targetTransform.position.z);
                    Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
                    targetPosition = targetPosition - directionToTarget * (myCollisionRadius + targetCollisionRadius + attackDistance / 2);

                    pathfinder.SetDestination(targetPosition);

                }
            }
            yield return new WaitForSeconds(followDelay);
        }
    }

}
