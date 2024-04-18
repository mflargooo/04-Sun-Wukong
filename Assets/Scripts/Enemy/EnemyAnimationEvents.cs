using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    public void DoAttack()
    {
        transform.root.GetComponent<Enemy>().DoAttack();
    }
}
