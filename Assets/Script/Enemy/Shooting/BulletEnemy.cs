using UnityEngine;

public class BulletEnemy : MonoBehaviour
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
        // ตรวจสอบว่ากระสุนชนกับผู้เล่นหรือไม่
        if (hitInfo.CompareTag("Player"))
        {
            Health playerHealth = hitInfo.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);  // สร้างความเสียหายให้ผู้เล่น
            }

            Destroy(gameObject);  // ทำลายกระสุนหลังจากชนกับผู้เล่น
        }

        // ตรวจสอบว่ากระสุนชนกับสิ่งกีดขวางหรือไม่
        if (hitInfo.CompareTag("Obstacle"))
        {
            Destroy(gameObject); // ทำลายกระสุนเมื่อชนกับสิ่งกีดขวาง
        }
    }
}
