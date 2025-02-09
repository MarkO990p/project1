using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;    // ความเร็วของกระสุน
    public int damage = 10;      // ค่าดาเมจของกระสุน
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;  // กระสุนจะเคลื่อนที่ไปข้างหน้า
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // ตรวจสอบว่ากระสุนชนกับศัตรูทั่วไป
        if (hitInfo.CompareTag("Enemy"))
        {
            Health enemyHealth = hitInfo.GetComponent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);  // สร้างความเสียหายให้ศัตรู
            }

            Destroy(gameObject); // ทำลายกระสุนหลังจากชนกับศัตรู
        }

        // ตรวจสอบว่ากระสุนชนกับบอส
        if (hitInfo.CompareTag("Boss"))
        {
            BossHealth bossHealth = hitInfo.GetComponent<BossHealth>();

            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);  // สร้างความเสียหายให้บอส
            }

            Destroy(gameObject); // ทำลายกระสุนหลังจากชนกับบอส
        }
        

        // ตรวจสอบว่ากระสุนชนกับสิ่งกีดขวางหรือไม่
        if (hitInfo.CompareTag("Obstacle"))
        {
            Destroy(gameObject); // ทำลายกระสุนเมื่อชนกับสิ่งกีดขวาง
        }
    }
}
