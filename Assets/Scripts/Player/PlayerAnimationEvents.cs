using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private Collider attackHitbox;

    public void PlayFootstep()
    {
        SoundManager.PlayFootstep();
    }

    public void PlayPlayerSwing()
    {
        SoundManager.PlayPlayerSwing();
    }

    public void SetPlayerAttackBoxActive(int b)
    {
        attackHitbox.enabled = b > 0;
    }

    public void PlayConnectedAttack()
    {
        SoundManager.PlayConnectedAttack();
    }
}
