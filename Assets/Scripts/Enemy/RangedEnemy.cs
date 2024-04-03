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

    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    protected override IEnumerator Attack()
    {
        proj = Instantiate(projectilePrefab, projSpawnLoc.position, projSpawnLoc.rotation);
        proj.Launch(proj.transform.forward);

        if (proj.TryGetComponent<AccelerateTowards>(out AccelerateTowards accelTowards))
        {
            accelTowards.SetTarget(player.transform);
        }

        yield return new WaitForSeconds(attackSpeed);
        EndAttack();
    }
}
