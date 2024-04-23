using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private string tagToDamage;
    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == tagToDamage && other.TryGetComponent<IDamageable>(out IDamageable dmg))
        {
            dmg.Damage(damage);
        }
    }
}
