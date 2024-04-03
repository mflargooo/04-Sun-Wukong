using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightProjectile : Projectile
{
    private Rigidbody rb;
    [SerializeField] private float launchSpeed;

    public override void Launch(Vector3 dir)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = dir * launchSpeed;
    }
}
