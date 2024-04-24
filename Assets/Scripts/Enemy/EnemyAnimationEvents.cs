using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private Collider attackHitBox;
    public void DoAttack()
    {
        transform.root.GetComponent<Enemy>().DoAttack();
    }

    public void SetAttackColliderActive(int i)
    {
        if (i == 0)
        {
            attackHitBox.enabled = false;
        }
        else
        {
            attackHitBox.enabled = true;
        }
    }

    public void PlaySwooshSound()
    {
        SoundManager.instance.PlayMeleeSwoosh();
    }

    public void PlayMetalSwoosh()
    {
        SoundManager.instance.PlayMetalSwoosh();
    }

    public void PlayThrowSound()
    {
        SoundManager.instance.PlayThrowSound();
    }

    public void PlayBowPullbackSound()
    {
        SoundManager.instance.PlayBowPullbackSound();
    }

    public void PlayBowReleaseSound()
    {
        SoundManager.instance.PlayBowReleaseSound();
    }
}
