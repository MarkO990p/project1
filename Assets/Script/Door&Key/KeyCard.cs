using UnityEngine;

public class Keycard : MonoBehaviour
{
    public string keycardID; // ID ของ Keycard ที่ต้องการ
    public Vector3 followOffset = new Vector3(0.5f, 1f, 0f); // Offset สำหรับตำแหน่งที่ต้องการให้กุญแจลอยตาม
    private bool isCollected = false; // สถานะว่ากุญแจถูกเก็บแล้วหรือยัง

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            isCollected = true; // ตั้งสถานะเป็นเก็บแล้ว
            AttachToPlayer(other.transform); // ตั้งให้กุญแจลอยตามผู้เล่น
        }
    }

    private void AttachToPlayer(Transform player)
    {
        // ทำให้กุญแจเป็นลูกของ Player เพื่อให้ลอยตาม
        transform.SetParent(player);
        transform.localPosition = followOffset; // ตั้งค่า Offset เพื่อให้กุญแจอยู่ในตำแหน่งที่ต้องการ
        GetComponent<Collider2D>().enabled = false; // ปิดการชนเพื่อป้องกันการชนซ้ำ
    }
}
