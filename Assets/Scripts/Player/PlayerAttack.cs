using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable dmg))
        {
            dmg.Damage(damage);
            SoundManager.instance.PlayConnectedAttack();
        }       

        if (other.TryGetComponent<IKnockbackable>(out IKnockbackable kb))
        {
            kb.Knockback((other.transform.position - transform.position - Vector3.up * (other.transform.position.y + transform.position.y)).normalized);
        }
    }
}
