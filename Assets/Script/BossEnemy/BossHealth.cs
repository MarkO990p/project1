using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 100;
    private Animator animator;
    private bool isInvulnerable = false;

    [Header("iFrames Settings")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private float numberOfFlashes;
    private SpriteRenderer spriteRend;
    private bool dead = false;

    [Header("Death Effect")]
    [SerializeField] private ParticleSystem deathEffect; // ใช้ ParticleSystem ตรง ๆ 

    private BossZoneTrigger bossZoneTrigger;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        bossZoneTrigger = FindObjectOfType<BossZoneTrigger>();
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || dead) return;

        health -= damage;
        Debug.Log("Boss took damage. Current Health: " + health);

        if (health <= 150)
        {
            animator.SetBool("IsEnraged", true);
        }

        if (health <= 0)
        {
            Die();
        }
        else
        {
            SetInvulnerability(true);
            StartCoroutine(InvulnerabilityCooldown());
            StartCoroutine(FlashDuringIFrames());
        }
    }

    private IEnumerator InvulnerabilityCooldown()
    {
        yield return new WaitForSeconds(iFramesDuration);
        SetInvulnerability(false);
    }

    private void Die()
    {
        if (dead) return;

        dead = true;
        animator.SetBool("die", true);
        Debug.Log("Boss is dead.");

        // แสดงเอฟเฟกต์การตายโดยตรงจาก ParticleSystem
        if (deathEffect != null)
        {
            ParticleSystem effect = Instantiate(deathEffect, transform.position, transform.rotation);
            effect.Play();
        }

        if (bossZoneTrigger != null)
        {
            bossZoneTrigger.OnBossDefeated();
        }

        Destroy(gameObject, 2f); // ทำลายบอสหลังจากแสดงเอฟเฟกต์
    }

    public void SetInvulnerability(bool value)
    {
        isInvulnerable = value;
        Debug.Log("Invulnerability set to: " + value);
    }

    private IEnumerator FlashDuringIFrames()
    {
        Color originalColor = spriteRend.color;
        Color flashColor = new Color(1, 0, 0, 0.5f);

        float flashInterval = iFramesDuration / (numberOfFlashes * 2);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            spriteRend.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRend.color = originalColor;
    }
}
