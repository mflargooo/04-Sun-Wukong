using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] protected LayerMask destroyOnContact;
    [SerializeField] protected LayerMask damageOnContact;
    public virtual void Launch(Vector3 launchVector) { }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageOnContact) != 0)
        {
            other.GetComponent<IDamageable>().Damage(damage);
        }

        if (((1 << other.gameObject.layer) & destroyOnContact) != 0)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Destroy(gameObject, 10f);
    }
}
