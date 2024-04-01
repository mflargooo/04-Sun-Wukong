using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private Projectile projectile;
    // Start is called before the first frame update

    Vector3 homePos;

    void Start()
    {
        homePos = transform.position;
        StartCoroutine(Wander())
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
