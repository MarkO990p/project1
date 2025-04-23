#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using UnityEngine;

public class DoorCon : MonoBehaviour
{
    public Vector3 closedPosition;          // ตำแหน่งเมื่อประตูปิด
    private Vector3 openPosition;           // ตำแหน่งเมื่อประตูเปิด (ตอนเริ่มเกม)

    public float moveSpeed = 5f;            // ความเร็วในการเคลื่อนที่ของประตู
    private const float threshold = 0.01f;  // ค่าความแม่นยำในการเปรียบเทียบตำแหน่ง

    private void Start()
    {
        openPosition = transform.position; // เก็บตำแหน่งเปิดตอนเริ่ม
    }

    public IEnumerator CloseDoor()
    {
        Debug.Log("Closing door from " + transform.position + " to " + closedPosition);
        while (Vector3.Distance(transform.position, closedPosition) > threshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, closedPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = closedPosition; // ล็อคตำแหน่ง
        Debug.Log(gameObject.name + " closed.");
    }

    public IEnumerator OpenDoor()
    {
        Debug.Log("Opening door from " + transform.position + " to " + openPosition);
        while (Vector3.Distance(transform.position, openPosition) > threshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = openPosition; // ล็อคตำแหน่ง
        Debug.Log(gameObject.name + " opened.");
    }

#if UNITY_EDITOR
    [ContextMenu("Set Closed Position to Current")]
    private void SetClosedPositionToCurrent()
    {
        closedPosition = transform.position;
        Debug.Log("Closed Position set to: " + closedPosition);
    }
#endif
}
