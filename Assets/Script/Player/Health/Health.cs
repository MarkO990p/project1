using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDataPersistence
{
    [Header("Health & Armor")]
    [SerializeField] private float startingHealth;
    [SerializeField] private float startingArmor;
    [SerializeField] private bool isPlayer;
    public float currentHealth { get; private set; }
    public float currentArmor { get; private set; }

    private Animator anim;
    private bool dead;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private RespawnManager respawnManager;
    private SpriteRenderer spriteRenderer;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration = 0.5f;
    [SerializeField] private float numberOfFlashes = 3;

    [Header("Death Effect")]
    [SerializeField] private ParticleSystem deathEffect;

    private void Awake()
    {
        currentHealth = startingHealth;
        currentArmor = startingArmor;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (isPlayer)
        {
            playerController = GetComponent<PlayerController>();
            respawnManager = FindObjectOfType<RespawnManager>();
        }
    }

    public void LoadData(GameData data)
    {
        this.currentHealth = data.currentHealth;
        this.currentArmor = data.currentArmor;
    }

    public void SaveData(GameData data)
    {
        data.currentHealth = this.currentHealth;
        data.currentArmor = this.currentArmor;
    }

    public void TakeDamage(float damage)
    {
        if (dead) return;

        float remainingDamage = damage;

        if (currentArmor > 0)
        {
            float absorbedDamage = Mathf.Min(remainingDamage, currentArmor);
            currentArmor -= absorbedDamage;
            remainingDamage -= absorbedDamage;
        }

        if (remainingDamage > 0)
        {
            currentHealth = Mathf.Clamp(currentHealth - remainingDamage, 0, startingHealth);
            StartCoroutine(FlashDuringIFrames());

            if (currentHealth <= 0 && !dead)
            {
                Die();
            }
        }
    }

    private IEnumerator FlashDuringIFrames()
    {
        Color originalColor = spriteRenderer.color;
        Color flashColor = new Color(1, 0, 0, 0.5f);
        float flashInterval = iFramesDuration / (numberOfFlashes * 2);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        if (dead) return;

        dead = true;
        anim.SetTrigger("die");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            deathEffect.Play();
        }

        if (isPlayer)
        {
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }

            GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
            if (gameOverManager != null)
            {
                gameOverManager.TriggerGameOver();
            }

            DisableAllComponents();
        }
        else
        {
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }
            Destroy(gameObject, 1f);
        }
    }

    private void DisableAllComponents()
    {
        MonoBehaviour[] components = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            if (component != this && component != playerController)
            {
                component.enabled = false;
            }
        }
    }

    public float GetCurrentArmor() => currentArmor;
    public float GetMaxArmor() => startingArmor;
    public float GetCurrentHealth() => currentHealth;

    public void SetCurrentHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, startingHealth);
    }

    public void ResetHealth()
    {
        currentHealth = startingHealth;
        currentArmor = startingArmor;
        dead = false;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (isPlayer && playerController != null)
        {
            playerController.enabled = true;
        }
    }

    public void AddHealth(int val)
    {
        if (dead) return;
        currentHealth = Mathf.Clamp(currentHealth + val, 0, startingHealth);
    }
}
