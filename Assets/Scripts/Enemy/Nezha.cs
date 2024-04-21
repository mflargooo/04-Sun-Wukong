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

    [SerializeField] private AnimationClip[] meleeAttacks;
    [SerializeField] private NezhaProjectile throwProjectilePrefab;
    [SerializeField] private Transform spawnProjLoc;

    [SerializeField] private GameObject boomerang;

    [SerializeField] private float chaseSpeed;
    [SerializeField] protected float meleeAttackRange;
    [SerializeField] protected float rangedAttackRange;
    [SerializeField] protected float attackCooldownFromMelee;
    [SerializeField] protected float attackCooldownFromRange;

    protected float health;

    protected Coroutine state;
    private Coroutine isAttacking;

    private bool canAttack = true;

    NezhaProjectile proj;

    private void Awake()
    {
        transform.position = Vector3.up * 1.335f;
        health = maxHealth;
        player = FindObjectOfType<IsometricPlayerController3D>();

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
        agent.speed = chaseSpeed;
        while (true)
        {
            if (health / maxHealth <= .75f)
            {
                NextState(PhaseTwo());
            }
            lockOnAngle = Vector3.SignedAngle(transform.forward, enemyToPlayer, Vector3.up);

            agent.SetDestination(player.transform.position);
            if (enemyToPlayer.magnitude > meleeAttackRange)
            {
                agent.updateRotation = true;
            }
            else
            {
                agent.updateRotation = false;
                if (Mathf.Abs(lockOnAngle) > 2f) transform.Rotate(transform.up, lockOnAngle * 5f * Time.deltaTime);
                else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
            }

            if (isAttacking == null && lockOnAngle < .2f && enemyToPlayer.magnitude <= meleeAttackRange && canAttack)
            {
                agent.SetDestination(transform.position);
                isAttacking = StartCoroutine(Attack(attackCooldownFromMelee, 0, 1));
            }

            yield return isAttacking;
        }
    }

    private IEnumerator PhaseTwo()
    {
        agent.enabled = true;
        print("STARTING PHASE TWO");

        float lockOnAngle = 0f;
        agent.speed = chaseSpeed;
        while (true)
        {
            if (health / maxHealth <= .5f)
            {
                NextState(PhaseThree());
            }
            lockOnAngle = Vector3.SignedAngle(transform.forward, enemyToPlayer, Vector3.up);

            agent.SetDestination(player.transform.position);
            if (enemyToPlayer.magnitude > meleeAttackRange)
            {
                agent.updateRotation = true;
            }
            else
            {
                agent.updateRotation = false;
                if (Mathf.Abs(lockOnAngle) > 2f) transform.Rotate(transform.up, lockOnAngle * 5f * Time.deltaTime);
                else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
            }

            if (isAttacking == null && lockOnAngle < .2f && canAttack)
            {
                if (enemyToPlayer.magnitude <= meleeAttackRange)
                {
                    agent.SetDestination(transform.position);
                    isAttacking = StartCoroutine(Attack(attackCooldownFromMelee, 0, 1));
                }
                else if(enemyToPlayer.magnitude <= rangedAttackRange)
                {
                    agent.SetDestination(transform.position);
                    isAttacking = StartCoroutine(Attack(attackCooldownFromRange, 1, 1));
                }
            }

            yield return isAttacking;
        }
    }

    private IEnumerator PhaseThree()
    {
        print("STARTING PHASE THREE");

        agent.speed = chaseSpeed;
        while (true)
        {
            yield return null;
        }
    }

    /* Attack 0 is basic melee combo, 1 is throw projectile, 2 is */
    /* Phases are 1-indexed */
    private IEnumerator Attack(float attackSpeed, int type, int phase)
    {
        canAttack = false;
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
                SummonProjectile(1);
                yield return new WaitForSeconds(.5f);
                break;
            case 2:
                break;
        }

        agent.enabled = true;
        StartCoroutine(DoAttackCooldown(attackSpeed));
        EndAttack();
    }

    private void SummonProjectile(int type)
    {
        proj = Instantiate(throwProjectilePrefab, spawnProjLoc.position, throwProjectilePrefab.transform.rotation);
        proj.BoomerangTo(spawnProjLoc, player.transform.position + Vector3.up * (spawnProjLoc.position.y - player.transform.position.y), type);
    }

    IEnumerator DoAttackCooldown(float attackSpeed)
    {
        yield return new WaitForSeconds(attackSpeed);
        canAttack = true;
    }

    void EndAttack()
    {
        if (isAttacking == null) return;
        StopCoroutine(isAttacking);
        isAttacking = null;
    }
}
