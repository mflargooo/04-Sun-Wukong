using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private AnimationClip attackAnim;
    protected override IEnumerator Attack()
    {
        anim.Play("attack");
        yield return new WaitForSeconds(attackAnim.length);
        EndAttack();
    }
}
