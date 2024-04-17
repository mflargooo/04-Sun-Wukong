using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    private float currentHealth;

    public void Damage(float damage)
    {
        currentHealth -= damage;
    }
}
