using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    private float health;

    void Start()
    {
        health = maxHealth;
    }

    public void Damage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            End();
        }
    }

    public void End() 
    {
        Destroy(gameObject);
    }
}
