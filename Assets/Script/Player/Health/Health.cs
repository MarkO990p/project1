using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health & Armor")]
    [SerializeField] private float startingHealth;
    [SerializeField] private float startingArmor;
    [SerializeField] private bool isPlayer;
    public float currentHealth { get; private set; }
    public float currentArmor { get; private set; }
    private bool shieldActive;

    [Header("Shield UI Images")]
    [SerializeField] private Image shieldActiveImage;
    [SerializeField] private Image shieldInactiveImage;

    [Header("Armor GameObject")]
    [SerializeField] private GameObject armorGameObject;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private float numberOfFlashes;
    private Animator anim;
    private bool dead;

    private PlayerController playerController;
    private Rigidbody2D rb;
    private RespawnManager respawnManager;

    // เพิ่ม ParticleSystem สำหรับเอฟเฟกต์ตอนตาย
    [Header("Death Effect")]
    [SerializeField] private ParticleSystem deathEffect;

    private void Awake()
    {
        currentHealth = startingHealth;
        currentArmor = startingArmor;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (isPlayer)
        {
            playerController = GetComponent<PlayerController>();
            respawnManager = FindObjectOfType<RespawnManager>();
        }

        UpdateShieldImage();
    }

    private void Update()
    {
        if (isPlayer && Input.GetKeyDown(KeyCode.L))
        {
            ToggleShield();
        }
    }

    private void ToggleShield()
    {
        shieldActive = !shieldActive;
        UpdateShieldImage();

        if (shieldActive)
        {
            Debug.Log("เปิดใช้งานเกราะ");
            if (armorGameObject != null)
            {
                armorGameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("ปิดการใช้งานเกราะ");
            if (armorGameObject != null)
            {
                armorGameObject.SetActive(false);
            }
        }
    }

    private void UpdateShieldImage()
{
    // ปิดการแสดงผลของเกราะหากเกราะหมด
    if (currentArmor <= 0)
    {
        shieldActive = false;
    }

    if (shieldActive)
    {
        if (shieldActiveImage != null) shieldActiveImage.enabled = true;
        if (shieldInactiveImage != null) shieldInactiveImage.enabled = false;
        if (armorGameObject != null) armorGameObject.SetActive(true);
    }
    else
    {
        if (shieldActiveImage != null) shieldActiveImage.enabled = false;
        if (shieldInactiveImage != null) shieldInactiveImage.enabled = true;
        if (armorGameObject != null) armorGameObject.SetActive(false);
    }
}

    public bool IsShieldActive()
    {
        return shieldActive;
    }

    public void TakeArmorDamage(float damage)
{
    if (shieldActive && currentArmor > 0)
    {
        float damageToArmor = Mathf.Min(damage, currentArmor);
        currentArmor -= damageToArmor;
        Debug.Log("Armor took " + damageToArmor + " damage. Remaining armor: " + currentArmor);
        
        // ตรวจสอบว่าเกราะหมดหรือไม่
        if (currentArmor <= 0 && armorGameObject != null)
        {
            Debug.Log("Armor is depleted. Deactivating armor GameObject.");
            armorGameObject.SetActive(false); // ปิดการแสดงผล GameObject ของเกราะ
            UpdateShieldImage(); // อัพเดต UI เพื่อแสดงให้แน่ใจว่าเกราะถูกปิด
        }
    }
}

    public void TakeHealthDamage(float damage)
    {
        if (!shieldActive || currentArmor <= 0)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
            Debug.Log("Health took " + damage + " damage. Remaining health: " + currentHealth);

            if (currentHealth <= 0 && !dead)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        if (dead) return;

        dead = true;
        anim.SetTrigger("die");

        // แสดง Particle Effect ตอนตาย
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

    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(3, 7, true);

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
        {
            Debug.LogWarning("SpriteRenderer not found on this GameObject.");
            yield break;
        }

        Color originalColor = sprite.color;
        Color flashColor = Color.red;

        for (int i = 0; i < numberOfFlashes; i++)
        {
            sprite.color = flashColor;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            sprite.color = originalColor;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }

        sprite.color = originalColor;
        Physics2D.IgnoreLayerCollision(3, 7, false);
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

    public void TakeDamage(float damage)
{
    if (dead) return;

    if (shieldActive && currentArmor > 0)
    {
        TakeArmorDamage(damage); // เรียกใช้ TakeArmorDamage ก่อน
    }
    
    // คำนวณความเสียหายต่อสุขภาพถ้าเกราะหมด
    if (damage > 0 && (!shieldActive || currentArmor <= 0))
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        Debug.Log("Health took " + damage + " damage. Remaining health: " + currentHealth);

        anim.SetTrigger("hurt");

        if (currentHealth <= 0 && !dead)
        {
            Die();
        }
        else
        {
            StartCoroutine(Invulnerability());
        }
    }
}
public float GetCurrentHealth()
{
    return currentHealth;
}

public void SetCurrentHealth(float health)
{
    currentHealth = Mathf.Clamp(health, 0, startingHealth);  // จำกัดค่าให้อยู่ในช่วงที่กำหนด
    Debug.Log("Loaded Health: " + currentHealth);
}

}
