using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private Slider healthBar;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;  
        UpdateHealthBar();
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.value = currentHealth / maxHealth;
    }
}
