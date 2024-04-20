using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private string tagToDamage;
    [SerializeField] private LayerMask layerToDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == tagToDamage && other.TryGetComponent<IDamageable>(out IDamageable dmg))
        {
            dmg.Damage(damage);
        }
    }
}
