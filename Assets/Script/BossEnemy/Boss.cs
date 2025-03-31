using UnityEngine;

public class Boss : MonoBehaviour
{
    private bool isActive = false;

    // Property เพื่อเข้าถึงตัวแปร isActive
    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    public Transform player;  // อ้างอิงถึงผู้เล่น
    public bool isFlipped = false;  // กำหนดสถานะพลิกทิศทาง

    public float moveSpeed = 2f;
    public float detectionRange = 10f;
    private bool isPatrolling = false;

    private Rigidbody2D rb;

    public LayerMask obstructionLayer;  // สิ่งกีดขวางที่ไม่ให้บอสเดิน

    void Start()
    {
        // ค้นหาผู้เล่นใน Scene ถ้ายังไม่ได้ตั้งค่า
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;  // ค้นหาผู้เล่นที่มี Tag "Player"
        }

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsActive)
        {
            LookAtPlayer();  // ให้บอสหันหน้าไปหาผู้เล่น

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                AttackPlayer();
            }
            else
            {
                Patrol();
            }
        }
    }

    // ฟังก์ชันเปิดใช้งานบอส
    public void ActivateBoss()
    {
        IsActive = true;
    }

    // ฟังก์ชันโจมตีผู้เล่น
    private void AttackPlayer()
    {
        Debug.Log("Boss is attacking the player!");
    }

    // ฟังก์ชันให้บอสหันหน้าไปหาผู้เล่น
    public void LookAtPlayer()
    {
        Vector3 scale = transform.localScale;

        // เช็คว่าผู้เล่นอยู่ทางซ้ายหรือขวาของบอส
        if (transform.position.x > player.position.x && scale.x > 0)  // ถ้าผู้เล่นอยู่ทางซ้าย
        {
            scale.x *= -1;  // พลิกไปทางซ้าย
            Debug.Log("Flipping to Left");
        }
        else if (transform.position.x < player.position.x && scale.x < 0)  // ถ้าผู้เล่นอยู่ทางขวา
        {
            scale.x *= -1;  // พลิกไปทางขวา
            Debug.Log("Flipping to Right");
        }

        transform.localScale = scale;  // อัปเดต localScale
    }

    // ฟังก์ชันเดินวนไปมาของบอส
    private void Patrol()
    {
        if (!isPatrolling)
        {
            isPatrolling = true;

            // ตรวจสอบขอบของพื้นที่
            if (transform.position.x >= 10f || transform.position.x <= -10f)
            {
                isFlipped = !isFlipped;
                transform.Rotate(0f, 180f, 0f);  // พลิกทิศทาง
            }

            // เดินไปข้างหน้า
            Vector2 moveDirection = isFlipped ? Vector2.left : Vector2.right;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    // เมื่อผู้เล่นเข้ามาในพื้นที่บอส
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the boss area.");
            ActivateBoss();  // เปิดใช้งานบอส
        }
    }
}
