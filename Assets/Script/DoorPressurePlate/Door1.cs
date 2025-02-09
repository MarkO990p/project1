using UnityEngine;
using System.Collections;

public class Door1 : MonoBehaviour
{
    private Vector3 closedPosition;
    public Vector3 openPosition;
    private bool isOpen = false;
    public float moveSpeed = 2f; // ความเร็วในการเปิดปิดประตู

    private void Start()
    {
        closedPosition = transform.position;
    }

    public void Open()
    {
        if (!isOpen)
        {
            StopAllCoroutines(); // หยุดคอร์รอตีนก่อนหน้า (ถ้ามี)
            StartCoroutine(MoveDoor(openPosition)); // เริ่มเคลื่อนประตูไปยังตำแหน่งเปิด (ขึ้นไป)
            isOpen = true;
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            StopAllCoroutines(); // หยุดคอร์รอตีนก่อนหน้า (ถ้ามี)
            StartCoroutine(MoveDoor(closedPosition)); // เริ่มเคลื่อนประตูไปยังตำแหน่งปิด (ลงมา)
            isOpen = false;
        }
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null; // รอเฟรมถัดไป
        }

        transform.position = targetPosition; // ตั้งตำแหน่งเป็นเป้าหมายเมื่อใกล้มากพอ
    }
}
