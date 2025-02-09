using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Door1 door; // เชื่อมกับ Door
    private bool isPressed = false; // ตรวจสอบว่าปุ่มถูกเหยียบอยู่หรือไม่
    public Vector3 pressedPosition; // ตำแหน่งเมื่อปุ่มถูกเหยียบ
    public Vector3 originalPosition; // ตำแหน่งเริ่มต้นของปุ่ม
    public float pressDistance = 0.1f; // ระยะการกดลงของปุ่ม
    private bool doorOpen = false; // ตรวจสอบว่าประตูเปิดอยู่หรือไม่
    public Color originalColor = Color.white; // สีเริ่มต้นของปุ่ม
    public Color pressedColor = Color.red; // สีเมื่อปุ่มถูกเหยียบ
    private SpriteRenderer spriteRenderer; // สำหรับการเปลี่ยนสีปุ่ม

    private void Start()
    {
        originalPosition = transform.position; // บันทึกตำแหน่งเริ่มต้นของปุ่ม
        pressedPosition = originalPosition - new Vector3(0, pressDistance, 0); // กำหนดตำแหน่งเมื่อปุ่มถูกเหยียบ
        spriteRenderer = GetComponent<SpriteRenderer>(); // เข้าถึง SpriteRenderer
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor; // ตั้งค่าสีเริ่มต้น
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isPressed) // ถ้ายังไม่ถูกเหยียบ
            {
                // สลับสถานะของประตู
                if (door != null)
                {
                    if (doorOpen)
                    {
                        door.Close(); // ปิดประตูถ้ามันเปิดอยู่
                    }
                    else
                    {
                        door.Open(); // เปิดประตูถ้ามันปิดอยู่
                    }
                    doorOpen = !doorOpen; // สลับสถานะของประตู
                }

                transform.position = pressedPosition; // เปลี่ยนตำแหน่งปุ่มไปยังตำแหน่งเหยียบ
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = pressedColor; // เปลี่ยนสีปุ่มเมื่อถูกเหยียบ
                }
                isPressed = true; // ตั้งค่าว่าปุ่มถูกเหยียบแล้ว
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPressed)
        {
            // ปล่อยปุ่มกลับสู่ตำแหน่งเดิม
            transform.position = originalPosition;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor; // เปลี่ยนสีปุ่มกลับไปเป็นสีเดิม
            }
            isPressed = false; // ตั้งค่าปุ่มเป็นไม่ถูกเหยียบ
        }
    }
}
