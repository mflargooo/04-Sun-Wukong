using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : Enemy
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projSpawnLoc;

    private Projectile proj;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    public void SpawnProjectile()
    {
        Vector3 centralize = Vector3.Cross(transform.forward, transform.up).normalized;
        proj = Instantiate(projectilePrefab, projSpawnLoc.position + centralize * .25f - transform.up * .5f, transform.rotation);
        proj.Launch(proj.transform.forward);

        if (proj.TryGetComponent<AccelerateTowards>(out AccelerateTowards accelTowards))
        {
            accelTowards.SetTarget(player.transform);
        }
    }

    public override void DoAttack()
    {
        SpawnProjectile();
    }

    protected override IEnumerator Attack()
    {
        anim.Play("attack");
        yield return new WaitForSeconds(attackSpeed);
        EndAttack();
    }
}
