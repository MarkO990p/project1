using UnityEngine;

public class PushPlayer : MonoBehaviour
{
    public float pushForce = 5f;        // แรงผลัก
    public float detectionRange = 5f;   // ระยะตรวจจับ

    private Transform player;
    private Rigidbody2D playerRb;

    void Start()
    {
        // ค้นหาผู้เล่นในเกม (แท็กของผู้เล่นต้องตั้งเป็น "Player")
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRb = player.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (player == null) return; // ถ้าไม่มีผู้เล่นในเกม ให้ออกจากฟังก์ชัน

        // ตรวจสอบว่าผู้เล่นอยู่ในระยะการตรวจจับหรือไม่
        if (Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            Push();
        }
    }

    void Push()
    {
        // คำนวณทิศทางจากมอนสเตอร์ไปยังผู้เล่น
        Vector2 direction = (player.position - transform.position).normalized;
        
        // เพิ่มแรงผลักไปยัง Rigidbody2D ของผู้เล่น
        playerRb.AddForce(direction * pushForce, ForceMode2D.Impulse);
    }
}
