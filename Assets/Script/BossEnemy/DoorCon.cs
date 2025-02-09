using System.Collections;
using UnityEngine;

public class DoorCon : MonoBehaviour
{
    public Vector3 closedPosition;    // ตำแหน่งที่ต้องการให้ประตูปิด
    private Vector3 openPosition;     // ตำแหน่งเปิดเริ่มต้นของประตู

    public float moveSpeed = 5f;      // ความเร็วในการเคลื่อนที่ของประตู

    private void Start()
    {
        openPosition = transform.position; // บันทึกตำแหน่งเปิดเริ่มต้นของประตู
    }

    public IEnumerator CloseDoor()
    {
        while (transform.position != closedPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, closedPosition, moveSpeed * Time.deltaTime);
            yield return null; // รอให้ผ่านไปหนึ่งเฟรมแล้วทำต่อ
        }
        Debug.Log(gameObject.name + " closed.");
    }

    public IEnumerator OpenDoor()
    {
        while (transform.position != openPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, moveSpeed * Time.deltaTime);
            yield return null; // รอให้ผ่านไปหนึ่งเฟรมแล้วทำต่อ
        }
        Debug.Log(gameObject.name + " opened.");
    }
}
