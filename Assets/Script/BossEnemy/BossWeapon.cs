using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public int attackDamage = 20;
    public LayerMask playerLayer;

    private float attackCooldown = 1.5f;
    private float lastAttackTime = -Mathf.Infinity;

    private BoxCollider2D attackCollider; // ใช้ BoxCollider2D แทนการกำหนดขนาดเอง

    void Awake()
    {
        // ตรวจสอบและเพิ่ม BoxCollider2D หากไม่มี
        attackCollider = GetComponent<BoxCollider2D>();
        if (attackCollider == null)
        {
            attackCollider = gameObject.AddComponent<BoxCollider2D>();
            attackCollider.isTrigger = true; // ตั้งค่าให้เป็น Trigger
        }
    }

    void Update()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Collider2D playerInRange = Physics2D.OverlapBox(attackCollider.bounds.center, attackCollider.bounds.size, 0f, playerLayer);
            if (playerInRange != null)
            {
                Health playerHealth = playerInRange.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackCollider.bounds.center, attackCollider.bounds.size);
        }
    }
}
