using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking };
    State currentState;

    public ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;
    Material skinSharedMaterial;

    Color originalColor;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1;
    float damage = 1;
    float sqrAttackDistanceThreshold;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start()
    {
        base.Start();

        if (hasTarget) {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;

            sqrAttackDistanceThreshold = Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2);

            StartCoroutine("UpdatePath");
        }
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColour) {
        pathfinder.speed = moveSpeed;

        if (hasTarget) {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;
        skinSharedMaterial = GetComponent<Renderer>().sharedMaterial;
        skinSharedMaterial.color = skinColour;
        originalColor = skinSharedMaterial.color;
        skinMaterial = GetComponent<Renderer>().material;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health && !dead) {
            if (OnDeathStatic != null) {
                OnDeathStatic();
            }
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)), deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath() {
        hasTarget = false;
        currentState = State.Idle;
    }


    void Update()
    {
        if (hasTarget) {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < sqrAttackDistanceThreshold)
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine("Attack");
                }
            }
        }
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1) {

            if(percent >= .5f && !hasAppliedDamage){
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-(Mathf.Pow(percent, 2)) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        pathfinder.enabled = true;
        currentState = State.Chasing;
    }

    IEnumerator UpdatePath() {
        float refreshRate = 0.25f;

        while (hasTarget) {
            if (!dead && currentState == State.Chasing) {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                pathfinder.SetDestination(targetPosition);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
