using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject doorClosed; // ประตูปิด
    public GameObject doorOpen; // ประตูเปิด
    public string requiredKeycardID; // Keycard ที่ต้องการเพื่อเปิดประตู

    private void Start()
    {
        doorClosed.SetActive(true);
        doorOpen.SetActive(true); // ซ่อน doorOpen ตอนเริ่มเกม
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && PlayerHasKeycard(collision.transform))
        {
            OpenDoor();
            DestroyKeycard(collision.transform); // ทำลาย Keycard เมื่อเปิดประตู
        }
    }

    private bool PlayerHasKeycard(Transform player)
    {
        // ตรวจสอบว่าผู้เล่นมี Keycard ที่เป็นลูกของ Player หรือไม่
        foreach (Transform child in player)
        {
            Keycard keycard = child.GetComponent<Keycard>();
            if (keycard != null && keycard.keycardID == requiredKeycardID)
            {
                return true;
            }
        }
        return false;
    }

    private void OpenDoor()
    {
        doorClosed.SetActive(false); // ซ่อนประตูปิด
        doorOpen.SetActive(true); // แสดงประตูเปิด
    }

    private void DestroyKeycard(Transform player)
    {
        // ค้นหาและทำลาย Keycard ที่เป็นลูกของผู้เล่น
        foreach (Transform child in player)
        {
            Keycard keycard = child.GetComponent<Keycard>();
            if (keycard != null && keycard.keycardID == requiredKeycardID)
            {
                Destroy(keycard.gameObject); // ทำลาย Keycard
                break;
            }
        }
    }
}
