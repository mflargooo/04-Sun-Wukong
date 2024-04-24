using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    [SerializeField] private AnimationClip groundSpearAnim;
    [SerializeField] private GameObject geyserAttack;

    [SerializeField] private float chaseSpeed;
    [SerializeField] protected float meleeAttackRange;
    [SerializeField] protected float rangedAttackRange;
    [SerializeField] protected float maintainDistRange;
    [SerializeField] protected float attackCDOfMelee;
    [SerializeField] protected float attackCDOfSpear;
    [SerializeField] protected float attackCDOfRanged;
    [SerializeField] protected float attackCDOfGround;

    private bool canMeleeAttack = true;
    private bool canSpearAttack = true;
    private bool canRangedAttack = true;
    private bool canGroundAttack = true;

    private float attackCD = 0;

    protected float health;

    [SerializeField] protected Slider healthBar;

    protected Coroutine state;
    private Coroutine isAttacking;

    private NezhaProjectile proj;

    [SerializeField] private Renderer[] modelObjs;
    [SerializeField] private GameObject ragdoll;

    private void Awake()
    {
        transform.position = Vector3.up * 1.335f;
        health = maxHealth;
        player = FindObjectOfType<IsometricPlayerController3D>();

        enemyToPlayer = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z);
        rb = GetComponent<Rigidbody>();
        healthBar.value = 1;

        foreach (ParticleSystem ps in spinSpearParticles)
        {
            if (!ps.isStopped) ps.Stop();
        }
    }

    private void Update()
    {
        if (!player) return;
        enemyToPlayer = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z);
        if (agent.enabled)
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void Damage(float damage)
    {
        health -= damage;

        healthBar.value = health / maxHealth;
        if (health <= .1f)
        {
            End();
            return;
        }
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
       foreach (Renderer rend in modelObjs)
       {
           rend.enabled = false;
       }
        yield return new WaitForSeconds(.2f);
        foreach (Renderer rend in modelObjs)
        {
            rend.enabled = true;
        }
    }

    public void End()
    {
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        player.GetComponent<PlayerStats>().Win();
        Instantiate(ragdoll, transform.position, transform.rotation);
        FindObjectOfType<SpawnManager>().RemoveEnemy("Nezha");
        if (proj)
        {
            Destroy(proj.gameObject);
        }
        yield return null;
        Destroy(gameObject);
    }
    private void NextState(IEnumerator next)
    {
        StopCoroutine(state);
        state = StartCoroutine(next);
    }

    public void StartNezha()
    {
        state = StartCoroutine(PhaseOne());
    }

    private IEnumerator PhaseOne()
    {
        agent.enabled = true;
        print("STARTING PHASE ONE");

        float lockOnAngle = 0f;
        float mag = 0f;
        agent.speed = chaseSpeed;
        int attempts = 0;
        while(!agent.isOnNavMesh && attempts < 500)
        {
            agent.enabled = false;
            yield return null;
            agent.enabled = true;
        }
        while (player)
        {
            if (health / maxHealth <= .75f)
            {
                NextState(PhaseTwo());
            }
            mag = enemyToPlayer.magnitude;
            lockOnAngle = Vector3.SignedAngle(transform.forward, enemyToPlayer, Vector3.up);

            if (attackCD > 0f)
            {
                attackCD -= Time.deltaTime;
            }

            if (agent.enabled)
            {
                if (attackCD <= 0f)
                {
                    if (mag > meleeAttackRange)
                    {
                        agent.updateRotation = false;
                        if (Mathf.Abs(lockOnAngle) > .2f) transform.Rotate(transform.up, lockOnAngle * 20f * Time.deltaTime);
                        else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
                    }
                    else
                    {
                        agent.updateRotation = true;
                    }
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    agent.updateRotation = false;
                    if (Mathf.Abs(lockOnAngle) > .2f) transform.Rotate(transform.up, lockOnAngle * 20f * Time.deltaTime);
                    else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
                    agent.SetDestination(player.transform.position - enemyToPlayer.normalized * maintainDistRange);
                }
            }

            if (isAttacking == null && lockOnAngle <= .2f && attackCD <= 0f)
            {
                if (mag <= meleeAttackRange && canMeleeAttack)
                {
                    rb.velocity = Vector3.zero;
                    if (agent.enabled)
                        agent.SetDestination(transform.position);
                    isAttacking = StartCoroutine(Attack(0, 2));
                }
                else if (mag <= rangedAttackRange && !proj && canRangedAttack)
                {
                    rb.velocity = Vector3.zero;
                    if (agent.enabled)
                        agent.SetDestination(transform.position);
                    isAttacking = StartCoroutine(Attack(1, 2));
                }
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
        while (player)
        {
            if (health / maxHealth <= .5f)
            {
                NextState(PhaseThree());
            }
            mag = enemyToPlayer.magnitude;
            lockOnAngle = Vector3.SignedAngle(transform.forward, enemyToPlayer, Vector3.up);

            if (attackCD > 0f)
            {
                attackCD -= Time.deltaTime;
            }

            if (agent.enabled)
            {
                if (attackCD <= 0f)
                {
                    if (mag > meleeAttackRange)
                    {
                        agent.updateRotation = false;
                        if (Mathf.Abs(lockOnAngle) > .2f) transform.Rotate(transform.up, lockOnAngle * 20f * Time.deltaTime);
                        else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
                    }
                    else
                    {
                        agent.updateRotation = true;
                    }
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    agent.updateRotation = false;
                    if (Mathf.Abs(lockOnAngle) > .2f) transform.Rotate(transform.up, lockOnAngle * 20f * Time.deltaTime);
                    else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
                    agent.SetDestination(player.transform.position - enemyToPlayer.normalized * maintainDistRange);
                }
            }

            if (isAttacking == null && lockOnAngle <= .2f && attackCD <= 0f)
            {
                if (mag <= meleeAttackRange)
                {
                    if (Random.Range(0, 1) < .8f && canMeleeAttack)
                    {
                        rb.velocity = Vector3.zero;
                        if (agent.enabled)
                            agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(0, 3));
                    }
                    else if (canSpearAttack)
                    {
                        rb.velocity = Vector3.zero;
                        if (agent.enabled)
                            agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(2, 3));
                    }
                }
                else if (meleeAttackRange < mag && mag <= rangedAttackRange)
                {
                    if (Random.Range(0, 1) < .6f && !proj && canRangedAttack)
                    {
                        if (agent.enabled)
                            agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(1, 3));
                    }
                    else if (canSpearAttack)
                    {
                        rb.velocity = Vector3.zero;
                        if (agent.enabled)
                            agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(2, 3));
                    }
                }
            }

            yield return isAttacking;
        }
    }

    private IEnumerator PhaseThree()
    {
        print("STARTING PHASE THRHEE");

        float lockOnAngle = 0f;
        float mag = 0f;
        agent.speed = chaseSpeed;
        while (player)
        {
            mag = enemyToPlayer.magnitude;
            lockOnAngle = Vector3.SignedAngle(transform.forward, enemyToPlayer, Vector3.up);

            if (attackCD > 0f)
            {
                attackCD -= Time.deltaTime;
            }

            if (agent.enabled)
            {
                if (attackCD <= 0f)
                {
                    if (mag > meleeAttackRange)
                    {
                        agent.updateRotation = false;
                        if (Mathf.Abs(lockOnAngle) > .2f) transform.Rotate(transform.up, lockOnAngle * 20f * Time.deltaTime);
                        else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
                    }
                    else
                    {
                        agent.updateRotation = true;
                    }
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    agent.updateRotation = false;
                    if (Mathf.Abs(lockOnAngle) > .2f) transform.Rotate(transform.up, lockOnAngle * 20f * Time.deltaTime);
                    else transform.rotation = Quaternion.LookRotation(enemyToPlayer, transform.up);
                    agent.SetDestination(player.transform.position - enemyToPlayer.normalized * maintainDistRange);
                }
            }

            if (isAttacking == null && lockOnAngle <= .2f && attackCD <= 0f)
            {
                if (mag <= meleeAttackRange)
                {
                    if (Random.Range(0, 1) < .9f && canMeleeAttack)
                    {
                        rb.velocity = Vector3.zero;
                        if (agent.enabled)
                            agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(0, 3));
                    }
                    else if (canSpearAttack)
                    {
                        rb.velocity = Vector3.zero;
                        if (agent.enabled)
                            agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(2, 3));
                    }
                }
                else if (meleeAttackRange < mag && mag <= rangedAttackRange)
                {
                    float prob = Random.Range(0f, 1f);
                    if (prob < .5f && !proj && canRangedAttack)
                    {
                        if (agent.enabled)
                            agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(1, 3));
                    }
                    else if (prob < .75f && canSpearAttack)
                    {
                        rb.velocity = Vector3.zero;
                        if (agent.enabled)
                            agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(2, 3));
                    }
                    else if (canGroundAttack)
                    {
                        rb.velocity = Vector3.zero;
                        if (agent.enabled)
                            agent.SetDestination(transform.position);
                        isAttacking = StartCoroutine(Attack(3, 3));
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
                anim.Play("throw");
                yield return new WaitForSeconds(.05f);
                SummonProjectile();
                yield return new WaitForSeconds(.5f);
                StartCoroutine(DoRangedCooldown());
                break;
            case 2:
                canSpearAttack = false;
                anim.SetBool("DoSpinAttack", true);
                anim.Play("spin_start");
                yield return new WaitForSeconds(spinStartAnim.length);
                SoundManager.instance.PlaySpinningWhooshSound();
                yield return new WaitForSeconds(1f);
                foreach (ParticleSystem ps in spinSpearParticles)
                {
                    if (!ps.isPlaying && ps.isStopped) ps.Play();
                }
                yield return new WaitForSeconds(spinSpearTime - 1f);
                foreach (ParticleSystem ps in spinSpearParticles)
                {
                    if (!ps.isStopped && ps.isPlaying) ps.Stop();
                    ps.time = 0;
                }
                anim.SetBool("DoSpinAttack", false);

                StartCoroutine(DoSpearCooldown());
                break;
            case 3:
                canGroundAttack = false;
                anim.Play("phase_2_start");
                yield return new WaitForSeconds(groundSpearAnim.length * 2f);
                Instantiate(geyserAttack, player.transform.position - (Vector3.up * (player.transform.position.y - .01f)) + player.transform.GetChild(0).transform.forward * player.isometricInput.magnitude * 2f, Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)));
                StartCoroutine(DoGroundCooldown());
                break;
        }

        agent.enabled = true;
        EndAttack();
    }
    private void SummonProjectile()
    {
        proj = Instantiate(throwProjectilePrefab, spawnProjLoc.position, throwProjectilePrefab.transform.rotation);
        proj.BoomerangTo(spawnProjLoc, player.transform.position + Vector3.up * (spawnProjLoc.position.y - player.transform.position.y) + player.transform.GetChild(0).transform.forward * Mathf.Sqrt(enemyToPlayer.magnitude));
    }

    IEnumerator DoMeleeCooldown()
    {
        attackCD = 2f;
        yield return new WaitForSeconds(attackCDOfMelee);
        canMeleeAttack = true;
    }
    IEnumerator DoRangedCooldown()
    {
        attackCD = 1.5f;
        yield return new WaitForSeconds(attackCDOfRanged);
        canRangedAttack = true;
    }
    IEnumerator DoSpearCooldown()
    {
        attackCD = 3f;
        yield return new WaitForSeconds(attackCDOfSpear);
        canSpearAttack = true;
    }
    IEnumerator DoGroundCooldown()
    {
        attackCD = 3f;
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
