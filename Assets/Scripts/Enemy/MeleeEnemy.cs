using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    protected override IEnumerator Attack()
    {
        yield return null;
        EndAttack();
    }
}
