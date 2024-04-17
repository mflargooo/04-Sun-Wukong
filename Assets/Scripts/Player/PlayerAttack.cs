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
            SoundManager.PlayConnectedAttack();
        }       
    }
}
