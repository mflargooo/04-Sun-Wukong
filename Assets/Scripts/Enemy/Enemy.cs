using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private NavMeshAgent agent;
    protected IsometricPlayerController3D player;
    protected RaycastHit hit;
    protected bool isLOS;

    Vector3 enemyToPlayer;

    [SerializeField] protected float maxHealth;

    [Header("Wander")]
    [SerializeField] private float wanderRadius;
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float minWaitWander;
    [SerializeField] private float maxWaitWander;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float relocateSpeed;
    [SerializeField] private float maintainDistRange;
    [SerializeField] protected float aggroRange;
    [SerializeField] protected float relieveAggroTime;
    [SerializeField] protected float attackRange;
    [SerializeField] private float attackSpeed;

    private bool playerLastSeen;

    protected float health;

    protected Vector3 homePos;
    protected Coroutine state;
    public virtual void Start()
    {
        health = maxHealth;
        player = FindObjectOfType<IsometricPlayerController3D>();

        homePos = transform.position;
        state = StartCoroutine(Wander());
    }

    public virtual void Update()
    {
        enemyToPlayer = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z);
        
        isLOS = Physics.Raycast(transform.position, enemyToPlayer.normalized, out hit, aggroRange, (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Ground"))) && hit.collider.tag == "Player";
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

    protected IEnumerator Aggro()
    {
        float rat = relieveAggroTime;

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
                        transform.rotation = Quaternion.LookRotation(enemyToPlayer.normalized, Vector3.up);
                        agent.speed = relocateSpeed;
                    }
                    else
                    {
                        agent.updateRotation = true;
                        transform.rotation = Quaternion.LookRotation(enemyToPlayer.normalized, Vector3.up);
                        agent.SetDestination(transform.position);
                    }
                }
            }

            if (rat <= 0f)
                NextState(Wander());

            yield return null;
        }
    }

    protected virtual IEnumerator Attack()
    {
        Debug.Log("Attack");
        yield return new WaitForSeconds(attackSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + enemyToPlayer.normalized * aggroRange);
    }
}
