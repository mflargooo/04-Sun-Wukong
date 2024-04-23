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

    [SerializeField] private ParticleSystem[] spinSpearParticles;
    [SerializeField] private AnimationClip spinStartAnim;
    [SerializeField] private float spinSpearTime;

    [SerializeField] private float chaseSpeed;
    [SerializeField] protected float meleeAttackRange;
    [SerializeField] protected float rangedAttackRange;
    [SerializeField] protected float attackCDOfMelee;
    [SerializeField] protected float attackCDOfSpear;
    [SerializeField] protected float attackCDOfRanged;
    [SerializeField] protected float attackCDOfGround;

    private bool canMeleeAttack = true;
    private bool canSpearAttack = true;
    private bool canRangedAttack = true;
    private bool canGroundAttack = true;

    protected float health;

    protected Coroutine state;
    private Coroutine isAttacking;

    private NezhaProjectile proj;

    private void Awake()
    {
        transform.position = Vector3.up * 1.335f;
        health = maxHealth;
        player = FindObjectOfType<IsometricPlayerController3D>();

        enemyToPlayer = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z);
        rb = GetComponent<Rigidbody>();

        foreach (ParticleSystem ps in spinSpearParticles)
        {
            if (!ps.isStopped) ps.Stop();
        }

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
        float mag = 0f;
        agent.speed = chaseSpeed;
        while (true)
        {
            if (health / maxHealth <= .75f)
            {
                NextState(PhaseTwo());
            }
            mag = enemyToPlayer.magnitude;
            lockOnAngle = Vector3.SignedAngle(transform.forward, enemyToPlayer, Vector3.up);

            agent.SetDestination(player.transform.position);
            if (mag > meleeAttackRange)
            {
                agent.updateRotation = true;
            }
            else
            {
                agent.updateRotation = false;
                if (Mathf.Abs(lockOnAngle) > 2f) transform.Rotate(transform.up, lockOnAngle * 5f * Time.deltaTime);
                else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
            }

            if (isAttacking == null && lockOnAngle < .2f && mag <= meleeAttackRange && canMeleeAttack)
            {
                rb.velocity = Vector3.zero;
                agent.SetDestination(transform.position);
                isAttacking = StartCoroutine(Attack(0, 1));
            }

            yield return isAttacking;
        }
    }

    private IEnumerator PhaseTwo()
    {
        agent.enabled = true;
        print("STARTING PHASE TWO");

        float lockOnAngle = 0f;
        float mag = 0f;
        agent.speed = chaseSpeed;
        while (true)
        {
            if (health / maxHealth <= .5f)
            {
                NextState(PhaseThree());
            }
            mag = enemyToPlayer.magnitude;
            lockOnAngle = Vector3.SignedAngle(transform.forward, enemyToPlayer, Vector3.up);

            agent.SetDestination(player.transform.position);
            if (mag > meleeAttackRange)
            {
                agent.updateRotation = true;
            }
            else
            {
                agent.updateRotation = false;
                if (Mathf.Abs(lockOnAngle) > 2f) transform.Rotate(transform.up, lockOnAngle * 5f * Time.deltaTime);
                else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
            }

            if (isAttacking == null && lockOnAngle < .2f)
            {
                if (mag <= meleeAttackRange && canMeleeAttack)
                {
                    rb.velocity = Vector3.zero;
                    agent.SetDestination(transform.position);
                    isAttacking = StartCoroutine(Attack(0, 2));
                }
                else if(mag <= rangedAttackRange && !proj && canRangedAttack)
                {
                    rb.velocity = Vector3.zero;
                    agent.SetDestination(transform.position);
                    isAttacking = StartCoroutine(Attack(1, 2));
                }
            }

            yield return isAttacking;
        }
    }

    private IEnumerator PhaseThree()
    {
        agent.enabled = true;
        print("STARTING PHASE THRHEE");

        float lockOnAngle = 0f;
        float mag = 0f;
        agent.speed = chaseSpeed;
        while (true)
        {
            mag = enemyToPlayer.magnitude;
            lockOnAngle = Vector3.SignedAngle(transform.forward, enemyToPlayer, Vector3.up);

            agent.SetDestination(player.transform.position);
            if (mag > meleeAttackRange)
            {
                agent.updateRotation = true;
            }
            else
            {
                agent.updateRotation = false;
                if (Mathf.Abs(lockOnAngle) > 2f) transform.Rotate(transform.up, lockOnAngle * 5f * Time.deltaTime);
                else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
            }

            if (isAttacking == null && lockOnAngle < .2f)
            {
                if (mag <= meleeAttackRange)
                {
                    if (Random.Range(0, 1) < .6f && canMeleeAttack)
                    {
                        rb.velocity = Vector3.zero;
                        agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(0, 3));
                    }
                    else if (canSpearAttack)
                    {
                        rb.velocity = Vector3.zero;
                        agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(2, 3));
                    }
                }
                else if (meleeAttackRange < mag && mag <= rangedAttackRange)
                {
                    if (Random.Range(0, 1) < .6f && !proj && canRangedAttack)
                    {
                        agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(1, 3));
                    }
                }
            }

            yield return isAttacking;
        }
    }

    /* Attack 0 is basic melee combo, 1 is throw projectile, 2 is */
    /* Phases are 1-indexed */
    private IEnumerator Attack(int type, int phase)
    {
        switch (type)
        {
            case 0:
                canMeleeAttack = false;

                agent.enabled = false;
                rb.velocity = Vector3.zero;

                Vector3 attackDir = enemyToPlayer.normalized;

                anim.Play("melee_attack_0");
                rb.velocity = attackDir * meleeAttackRange * .75f;
                yield return new WaitForSeconds(meleeAttacks[0].length / .8f * 1.2f);
                rb.velocity = Vector3.zero;
                yield return new WaitForSeconds(.1f);

                anim.Play("melee_attack_1");
                rb.velocity = attackDir * meleeAttackRange * .75f;
                yield return new WaitForSeconds(meleeAttacks[1].length / .8f * 1.2f);
                rb.velocity = Vector3.zero;
                yield return new WaitForSeconds(.1f);

                anim.Play("melee_attack_2");
                rb.velocity = attackDir * meleeAttackRange * .75f;
                yield return new WaitForSeconds(meleeAttacks[2].length / .8f * 1.7f);
                StartCoroutine(DoMeleeCooldown());
                break;
            case 1:
                canRangedAttack = false;

                agent.enabled = false;
                anim.Play("throw");
                yield return new WaitForSeconds(.05f);
                SummonProjectile();
                yield return new WaitForSeconds(.5f);
                StartCoroutine(DoRangedCooldown());
                break;
            case 2:
                canSpearAttack = false;

                agent.enabled = false;
                anim.SetBool("DoSpinAttack", true);
                anim.Play("spin_start");
                yield return new WaitForSeconds(spinStartAnim.length);
                foreach (ParticleSystem ps in spinSpearParticles)
                {
                    if (!ps.isPlaying && ps.isStopped) ps.Play();
                }
                yield return new WaitForSeconds(spinSpearTime);
                foreach (ParticleSystem ps in spinSpearParticles)
                {
                    if (!ps.isStopped && ps.isPlaying) ps.Stop();
                }
                anim.SetBool("DoSpinAttack", false);

                StartCoroutine(DoSpearCooldown());
                break;
            case 3:
                break;
        }

        agent.enabled = true;
        EndAttack();
    }

    void OnDestroy()
    {
        FindObjectOfType<SpawnManager>().RemoveEnemy("Nezha");
    }

    private void SummonProjectile()
    {
        proj = Instantiate(throwProjectilePrefab, spawnProjLoc.position, throwProjectilePrefab.transform.rotation);
        proj.BoomerangTo(spawnProjLoc, player.transform.position + Vector3.up * (spawnProjLoc.position.y - player.transform.position.y) + player.transform.GetChild(0).transform.forward * Mathf.Sqrt(enemyToPlayer.magnitude));
    }

    IEnumerator DoMeleeCooldown()
    {
        yield return new WaitForSeconds(attackCDOfMelee);
        canMeleeAttack = true;
    }
    IEnumerator DoRangedCooldown()
    {
        yield return new WaitForSeconds(attackCDOfRanged);
        canRangedAttack = true;
    }
    IEnumerator DoSpearCooldown()
    {
        yield return new WaitForSeconds(attackCDOfSpear);
        canSpearAttack = true;
    }
    IEnumerator DoGroundCooldown()
    {
        yield return new WaitForSeconds(attackCDOfGround);
        canGroundAttack = true;
    }

    void EndAttack()
    {
        if (isAttacking == null) return;
        StopCoroutine(isAttacking);
        isAttacking = null;
    }
}
