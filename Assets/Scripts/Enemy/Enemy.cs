using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable, IKnockbackable
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] protected NavMeshAgent agent;
    protected IsometricPlayerController3D player;
    [SerializeField] protected Animator anim;

    protected RaycastHit hit;
    protected bool isLOS;

    protected Vector3 enemyToPlayer;

    [SerializeField] protected float maxHealth;

    [Header("Wander")]
    [SerializeField] protected float wanderRadius;
    [SerializeField] protected float wanderSpeed;
    [SerializeField] protected float minWaitWander;
    [SerializeField] protected float maxWaitWander;

    [Header("Chase")]
    [SerializeField] protected float chaseSpeed;
    [SerializeField] protected float relocateSpeed;
    [SerializeField] protected float maintainDistRange;
    [SerializeField] protected float aggroRange;
    [SerializeField] protected float relieveAggroTime;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackSpeed;

    [Header("Knockback")]
    [SerializeField] protected float knockbackSpeed;
    [SerializeField] protected float knockbackTime;

    [Header("ID")]
    private string id = "Enemy";

    protected bool playerLastSeen;

    protected float health;

    protected Vector3 homePos;
    protected Coroutine state;
    protected Coroutine isAttacking;
    protected Coroutine isKnockingback;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
        player = FindObjectOfType<IsometricPlayerController3D>();

        homePos = transform.position;
        state = StartCoroutine(Wander());
    }

    protected virtual void Update()
    {
        enemyToPlayer = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z);
        if (enemyToPlayer.magnitude > 2 * attackRange && isAttacking != null) EndAttack();
        isLOS = Physics.Raycast(transform.position, enemyToPlayer.normalized, out hit, aggroRange, (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Ground"))) && hit.collider.tag == "Player";
    }

    public void Knockback(Vector3 dir)
    {
        if (isKnockingback != null) return;

        isKnockingback = StartCoroutine(DoKnockback(dir));
    }

    IEnumerator DoKnockback(Vector3 dir)
    {
        float timer = 0;
        while(timer < knockbackTime)
        {
            rb.velocity = dir * knockbackSpeed;
            timer += Time.deltaTime;
            yield return null;
        }
        rb.velocity = Vector3.zero;

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

    protected void NextState(IEnumerator next)
    {
        StopCoroutine(state);
        state = StartCoroutine(next);
    }

    protected IEnumerator Wander()
    {
        float angle = 0;
        float range = 0;
        Vector3 target = homePos;

        agent.speed = wanderSpeed;

        while (true)
        {
            if ((target - transform.position).sqrMagnitude < .01f)
            {
                yield return new WaitForSeconds(Random.Range(minWaitWander, maxWaitWander));
                angle = Random.Range(0f, 2 * Mathf.PI);
                range = Random.Range(.5f, 1f) * wanderRadius;
                target = homePos + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * range;
            }
            else
            {
                agent.SetDestination(target);
            }

            if (isLOS)
            {
                NextState(Aggro());
            }

            yield return null;
        }
    }

    protected virtual IEnumerator Aggro()
    {
        float rat = relieveAggroTime;
        float lockOnAngle = 0f;
        while (true)
        {
            if (!isLOS)
            {
                if (!playerLastSeen)
                {
                    agent.SetDestination(player.transform.position);
                    playerLastSeen = true;
                }

                agent.updateRotation = true;
                rat -= Time.deltaTime;
            }
            else
            {
                playerLastSeen = false;
                rat = relieveAggroTime;
                lockOnAngle = Vector3.SignedAngle(transform.forward, enemyToPlayer, Vector3.up);

                if (enemyToPlayer.sqrMagnitude > attackRange * attackRange)
                {
                    agent.SetDestination(player.transform.position);
                    agent.updateRotation = true;
                    agent.speed = chaseSpeed;
                }
                else
                {
                    if (enemyToPlayer.sqrMagnitude < maintainDistRange * maintainDistRange)
                    {
                        agent.SetDestination(transform.position + enemyToPlayer.normalized * (enemyToPlayer.magnitude - maintainDistRange));
                        agent.updateRotation = false;

                        if (Mathf.Abs(lockOnAngle) >= .2f) transform.Rotate(transform.up, lockOnAngle * 2f * Time.deltaTime);
                        
                        agent.speed = relocateSpeed;
                        if (isAttacking == null && lockOnAngle < .2f) isAttacking = StartCoroutine(Attack());
                    }
                    else
                    {
                        agent.updateRotation = false;

                        if (Mathf.Abs(lockOnAngle) >= .2f) transform.Rotate(transform.up, lockOnAngle * 2f * Time.deltaTime);

                        agent.SetDestination(transform.position);

                        if (isAttacking == null && lockOnAngle < .2f) isAttacking = StartCoroutine(Attack());
                    }
                }
            }

            if (rat <= 0f)
                NextState(Wander());

            yield return null;
        }
    }

    private void OnDestroy()
    {
        FindObjectOfType<SpawnManager>().RemoveEnemy(id);
    }

    public virtual void DoAttack()
    {
        return;
    }

    protected virtual IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackSpeed);
        EndAttack();
    }

    public virtual void EndAttack()
    {
        StopCoroutine(isAttacking);
        isAttacking = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + enemyToPlayer.normalized * aggroRange);
    }
}
