using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private Slider healthBar;
    private float currentHealth;

    [SerializeField] private float invincibleTime;
    private bool isInvincible;

    [SerializeField] private Renderer[] modelObjs;
    [SerializeField] private Material playerMaterial;

    [SerializeField] private GameObject ragdoll;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject playerUIScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private Material wukong;

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
            if(currentHealth <= 0f)
            {
                StartCoroutine(End());
                return;
            }
            if (damage >= 1f)
            {
                StartCoroutine(InvincibleTimer());
            }
            StartCoroutine(DamageIndication());
            UpdateHealthBar();
        }
    }

    IEnumerator End()
    {
        GetComponent<PlayerStats>().Dead();
        wukong.color = Color.white;
        cam.transform.parent = null;
        cam.GetComponent<AudioSource>().Stop();
        gameOverScreen.transform.SetParent(null);
        playerUIScreen.transform.SetParent(null);
        gameOverScreen.transform.GetChild(1).gameObject.SetActive(true);
        gameOverScreen.SetActive(true);
        SceneManager.MoveGameObjectToScene(playerUIScreen, SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(gameOverScreen, SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(cam, SceneManager.GetActiveScene());
        Instantiate(ragdoll, transform.position, transform.GetChild(0).rotation);
        yield return null;
        Destroy(gameObject);
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
