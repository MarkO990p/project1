using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform Waypoint1, Waypoint2;
    public int Speed;
    Vector2 targetPos;

    private void Start()
    {
        targetPos = Waypoint1.position;
    }

    private void Update()
    {
        // เปลี่ยนทิศทางเมื่อถึงจุดหมาย
        if (Vector2.Distance(transform.position, Waypoint1.position) < 0.1f) targetPos = Waypoint2.position;
        if (Vector2.Distance(transform.position, Waypoint2.position) < 0.1f) targetPos = Waypoint1.position;

        // เคลื่อนที่แพลตฟอร์มไปยังตำแหน่งเป้าหมาย
        transform.position = Vector2.MoveTowards(transform.position, targetPos, Speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ถ้าผู้เล่นชนกับแพลตฟอร์ม ให้ผู้เล่นติดอยู่กับแพลตฟอร์ม
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ถ้าผู้เล่นออกจากแพลตฟอร์ม ให้ผู้เล่นหลุดจากการติดกับแพลตฟอร์ม
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
