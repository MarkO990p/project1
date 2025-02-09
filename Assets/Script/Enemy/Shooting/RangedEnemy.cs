using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown; // ความถี่ในการยิง
    [SerializeField] private float range; // ระยะการมองเห็นผู้เล่น
    [SerializeField] private GameObject bulletPrefab; // กระสุนที่ใช้ยิง
    [SerializeField] private Transform firePoint; // จุดยิงกระสุน

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    // References
    private Animator anim;
    private Health playerHealth;
    private EnemyPatrol enemyPatrol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // ยิงกระสุนเมื่อเห็นผู้เล่นและ cooldown พร้อม
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                // ยิงกระสุน
                cooldownTimer = 0;
                anim.SetTrigger("rangedAttack"); // เรียกใช้งาน Trigger สำหรับอนิเมชัน
                Shoot();
            }
        }

        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = !PlayerInSight();
        }
    }

    private bool PlayerInSight()
    {
        // ใช้ Physics2D.BoxCast เพื่อตรวจสอบว่าผู้เล่นอยู่ในระยะยิงหรือไม่
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
        }

        return hit.collider != null;
    }

    private void Shoot()
    {
        // สร้างกระสุนที่จุด firePoint และให้เคลื่อนที่ไปยังทิศทางของผู้เล่น
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // คำนวณทิศทางการยิง
        Vector2 direction = (playerHealth.transform.position - firePoint.position).normalized;
        rb.velocity = direction * 10f; // กำหนดความเร็วของกระสุน
    }

    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                                new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
        }
    }
}
