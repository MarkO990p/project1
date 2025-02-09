using UnityEngine;

public class ShowIconOnTrigger : MonoBehaviour
{
    public GameObject icon;  // อ้างอิงถึง GameObject ของไอคอน

    private void Start()
    {
        // ซ่อนไอคอนเริ่มต้น
        icon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าผู้เล่นเข้ามาในพื้นที่ทริกเกอร์
        if (other.CompareTag("Player"))
        {
            icon.SetActive(true);  // แสดงไอคอนเมื่อผู้เล่นอยู่ในระยะ
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // ตรวจสอบว่าผู้เล่นออกจากพื้นที่ทริกเกอร์
        if (other.CompareTag("Player"))
        {
            icon.SetActive(false);  // ซ่อนไอคอนเมื่อผู้เล่นออกจากระยะ
        }
    }
}
