using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private Slider healthBar;
    private float currentHealth;

    [SerializeField] private float invincibleTime;
    private bool isInvincible;

    [SerializeField] private Renderer[] modelObjs;
    [SerializeField] private Material playerMaterial;

    void Start()
    {
        currentHealth = maxHealth;  
        UpdateHealthBar();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HealthPickup" && currentHealth < maxHealth)
        {
            currentHealth += .1f * maxHealth;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            Destroy(other.transform.root.gameObject);
            SoundManager.instance.PlayHealthPickup();
            UpdateHealthBar();
        }
    }

    public void Damage(float damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            if (damage >= 1f)
            {
                StartCoroutine(InvincibleTimer());
            }
            StartCoroutine(DamageIndication());
            UpdateHealthBar();
        }
    }

    IEnumerator DamageIndication()
    {
        playerMaterial.color = new Color (0xFF / 255f, 0x92 / 255f, 0x92 / 255f);
        yield return new WaitForSeconds(.1f);
        playerMaterial.color = Color.white;

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds((invincibleTime - .1f) / 20);
            foreach (Renderer rend in modelObjs)
            {
                rend.enabled = false;
            }
            playerMaterial.color = Color.white;
            yield return new WaitForSeconds((invincibleTime - .1f) / 20);
            foreach (Renderer rend in modelObjs)
            {
                rend.enabled = true;
            }
        }

        foreach (Renderer rend in modelObjs)
        {
            rend.enabled = true;
        }
    }

    IEnumerator InvincibleTimer()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    private void UpdateHealthBar()
    {
        healthBar.value = currentHealth / maxHealth;
    }
}
