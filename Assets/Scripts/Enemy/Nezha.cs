using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nezha : MonoBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    private Rigidbody rb;
    protected IsometricPlayerController3D player;
    [SerializeField] protected Animator anim;

    Vector3 enemyToPlayer;

    [SerializeField] protected float maxHealth;

    [SerializeField]
    private AnimationClip[] meleeAttacks;

    [Header("Phase 1")]
    [SerializeField] private float p1ChaseSpeed;
    [SerializeField] protected float p1MeleeAttackRange;
    [SerializeField] protected float p1AttackCooldownFromMelee;

    [Header("Phase 2")]
    [SerializeField] private float p2ChaseSpeed;
    [SerializeField] protected float p2MeleeAttackRange;
    [SerializeField] protected float p2RangeAttackRange;
    [SerializeField] protected float p2AttackCooldownFromMelee;
    [SerializeField] protected float p2AttackCooldownFromRange;

    [Header("Phase 3")]
    [SerializeField] private float p3ChaseSpeed;
    [SerializeField] protected float p3MeleeAttackRange;
    [SerializeField] protected float p3RangeAttackRange;
    [SerializeField] protected float p3AttackCooldownFromMelee;
    [SerializeField] protected float p3AttackCooldownFromRange;

    protected float health;

    protected Vector3 homePos;
    protected Coroutine state;
    private Coroutine isAttacking;

    private bool canAttack = true;

    private void Start()
    {
        health = maxHealth;
        player = FindObjectOfType<IsometricPlayerController3D>();

        homePos = transform.position;
        enemyToPlayer = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z);
        rb = GetComponent<Rigidbody>();

        state = StartCoroutine(PhaseOne());
    }

    private void Update()
    {
        enemyToPlayer = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z);
        if (agent.enabled)
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void Damage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            End();
        }
    }

    public void End()
    {
        Destroy(gameObject);
    }

    private void NextState(IEnumerator next)
    {
        StopCoroutine(state);
        state = StartCoroutine(next);
    }

    private IEnumerator PhaseOne()
    {
        agent.enabled = true;
        print("STARTING PHASE ONE");

        float lockOnAngle = 0f;
        agent.speed = p1ChaseSpeed;
        while (true)
        {
            if (health / maxHealth <= .75f)
            {
                NextState(PhaseTwo());
            }
            lockOnAngle = Vector3.SignedAngle(transform.forward, enemyToPlayer, Vector3.up);

            agent.SetDestination(player.transform.position);
            if (enemyToPlayer.magnitude > p1MeleeAttackRange)
            {
                agent.updateRotation = true;
            }
            else
            {
                agent.updateRotation = false;
                if (Mathf.Abs(lockOnAngle) > 2f) transform.Rotate(transform.up, lockOnAngle * 5f * Time.deltaTime);
                else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
            }

            if (isAttacking == null && lockOnAngle < .2f && enemyToPlayer.magnitude <= p1MeleeAttackRange && canAttack)
            {
                agent.SetDestination(transform.position);
                isAttacking = StartCoroutine(Attack(p1AttackCooldownFromMelee, 0, 1));
            }

            yield return isAttacking;
        }
    }

    private IEnumerator PhaseTwo()
    {
        print("STARTING PHASE TWO");

        agent.speed = p2ChaseSpeed;
        while(true)
        {
            if (health / maxHealth <= .5f)
            {
                NextState(PhaseThree());
            }
            yield return null;
        }
    }

    private IEnumerator PhaseThree()
    {
        print("STARTING PHASE THREE");

        agent.speed = p2ChaseSpeed;
        while (true)
        {
            yield return null;
        }
    }

    /* Attack 0 is basic melee combo, 1 is throw projectile, 2 is */
    /* Phases are 1-indexed */
    private IEnumerator Attack(float attackSpeed, int type, int phase)
    {
        canAttack = true;
        switch (type)
        {
            case 0:
                agent.enabled = false;
                anim.Play("melee_attack_0");
                rb.velocity = enemyToPlayer.normalized * 1.5f;
                yield return new WaitForSeconds(meleeAttacks[0].length / .8f * 1.1f);
                anim.Play("melee_attack_1");
                rb.velocity = enemyToPlayer.normalized * 1.5f;
                yield return new WaitForSeconds(meleeAttacks[1].length / .8f * 1.1f);
                anim.Play("melee_attack_2");
                rb.velocity = enemyToPlayer.normalized * 1.5f;
                yield return new WaitForSeconds(meleeAttacks[2].length / .8f * 1.1f);
                break;
            case 1:
                agent.enabled = false;
                if (phase == 1)
                    anim.Play("throw_normal_proj");
                else 
                    anim.Play("throw_enhanced_proj")
                yield return new WaitForSeconds(throwNormProj);
                break;
            case 2:
                break;
        }

        agent.enabled = true;
        StartCoroutine(DoAttackCooldown(attackSpeed));
        yield return new WaitForSeconds(attackSpeed);
        EndAttack();
    }

    IEnumerator DoAttackCooldown(float attackSpeed)
    {
        yield return new WaitForSeconds(attackSpeed);
        canAttack = true;
    }

    private void EndAttack()
    {
        StopCoroutine(isAttacking);
        isAttacking = null;
    }
}
