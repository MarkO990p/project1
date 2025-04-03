using System.Collections;
using UnityEngine;

public class MaleAttack : MonoBehaviour
{
    public float attackRange = 0.5f;         // ระยะการโจมตี
    public float attackDamage = 10f;         // ความเสียหายที่ศัตรูได้รับ
    public Transform attackPoint;            // ตำแหน่งที่โจมตี
    public LayerMask enemyLayer;             // เลเยอร์ของศัตรู

    private Animator animator;
    private bool isAttacking;                // สถานะการโจมตี
    private bool isPowerUpActive;            // ตรวจสอบว่า Power-Up ใช้งานอยู่หรือไม่

    void Start()
    {
        animator = GetComponent<Animator>(); // ดึง Component Animator มาเพื่อควบคุมการเคลื่อนไหว
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && !isAttacking) // เมื่อกดปุ่ม K เพื่อโจมตี
        {
            Attack();
        }
    }

    void Attack()
    {
        // ตั้งค่า isAttacking ให้เป็น true เพื่อเริ่มต้นแอนิเมชัน
        isAttacking = true;
        animator.SetBool("isAttacking", true);

        // ตรวจจับศัตรูในระยะการโจมตี
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // ทำความเสียหายให้กับศัตรูที่อยู่ในระยะ
        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage); // เรียกฟังก์ชัน TakeDamage ของศัตรู
                Debug.Log("Hit " + enemy.name + " for " + attackDamage + " damage");
            }
        }

        // เริ่มต้นการคูลดาวน์ของการโจมตี
        StartCoroutine(EndAttack());
    }

    private IEnumerator EndAttack()
    {
        // รอให้แอนิเมชันการโจมตีจบลง (สมมติว่าใช้เวลา 0.5 วินาที)
        yield return new WaitForSeconds(0.5f);

        // ตั้งค่า isAttacking เป็น false และตั้งค่า Animator กลับไปเป็น false เพื่อหยุดแอนิเมชัน
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    // ฟังก์ชันเพิ่มพลังโจมตี
    public void AddAttackPower(int amount)
    {
        attackDamage += amount;  // เพิ่มความเสียหายในการโจมตี
        Debug.Log("Attack Power Boosted! New Attack Damage: " + attackDamage);
    }

    // ฟังก์ชันลบพลังโจมตี
    public void RemoveAttackPower(int amount)
    {
        attackDamage -= amount;  // คืนค่าความเสียหายในการโจมตี
        Debug.Log("Attack Power Expired. New Attack Damage: " + attackDamage);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        // วาดวงกลมใน Scene View เพื่อแสดงระยะการโจมตี
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void CancelAttackState()
    {
        isAttacking = false;
        isPowerUpActive = false;
        if (animator != null)
            animator.SetBool("isAttacking", false);
    }

}
