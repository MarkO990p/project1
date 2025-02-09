using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown = 2f; // ระยะเวลาคูลดาวน์ในการโจมตี
    [SerializeField] private float range = 1.5f;        // ระยะโจมตี
    [SerializeField] private int damage = 5;            // ค่าความเสียหายที่สร้างให้กับผู้เล่น
    [SerializeField] private float attackTime = 0.5f;   // เวลาที่ต้องรอก่อนทำดาเมจหลังเริ่มแอนิเมชันโจมตี

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance = 1f; // ระยะห่างของการตรวจจับ
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer; // เลเยอร์ของผู้เล่น
    private float cooldownTimer = Mathf.Infinity;

    // References
    private Animator anim;
    private Health playerHealth;
    private EnemyPatrol enemyPatrol;
    private bool isPlayerInSight = false;
    private bool hasDealtDamage = false; // ใช้ตรวจสอบว่ามีการทำดาเมจไปแล้วในรอบนั้นหรือยัง

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();

        // ตรวจสอบว่ามีการกำหนดค่า boxCollider หรือไม่
        if (boxCollider == null)
        {
            Debug.LogWarning("BoxCollider2D not assigned on " + gameObject.name);
        }
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        bool playerInSight = PlayerInSight();

        // เริ่มแอนิเมชันโจมตีเมื่อผู้เล่นอยู่ในระยะและคูลดาวน์ครบ
        if (playerInSight && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0;
            anim.SetTrigger("meleeAttack"); // เริ่มแอนิเมชันโจมตี
            hasDealtDamage = false; // รีเซ็ตเพื่อให้สามารถโจมตีในรอบนี้ได้
            Debug.Log("Player in sight. Triggering melee attack animation.");
        }

        // ตรวจสอบว่าเวลาที่กำหนดผ่านไปแล้วหรือยังเพื่อทำดาเมจ
        if (playerInSight && !hasDealtDamage && cooldownTimer >= attackTime)
        {
            DamagePlayer(); // ทำดาเมจให้ผู้เล่น
            hasDealtDamage = true; // ป้องกันการทำดาเมจซ้ำในรอบเดียว
        }

        // ควบคุมการเดินลาดตระเวนตามการมองเห็นผู้เล่น
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = !playerInSight;
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกใช้เพื่อทำดาเมจ
    private void DamagePlayer()
    {
        Debug.Log("DamagePlayer function called.");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);  // ลดพลังชีวิตของผู้เล่น
            Debug.Log("Player took " + damage + " damage.");
        }
        else
        {
            Debug.LogWarning("Player not in sight or playerHealth is null.");
        }
    }

    // ฟังก์ชันตรวจจับว่าผู้เล่นอยู่ในระยะโจมตีหรือไม่
    private bool PlayerInSight()
    {
        if (boxCollider == null) return false;

        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
            return true;
        }

        playerHealth = null;
        return false;
    }

    // แสดงขอบเขตของ BoxCast ใน Scene View เพื่อช่วยในการตั้งค่าระยะ
    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
        }
    }
}
