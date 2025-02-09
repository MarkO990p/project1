using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public string sceneToLoad; // ชื่อของ Scene ที่ต้องการย้ายไป
    public Vector3 customReturnPosition; // ตำแหน่งที่ผู้เล่นจะไปเกิดใน Scene ใหม่
    public GameObject player; // ผู้เล่นที่ต้องเปลี่ยนตำแหน่งเมื่อเปลี่ยน Scene
    public KeyCode interactKey = KeyCode.E; // ปุ่มที่ใช้ในการกดเพื่อย้าย Scene
    private bool isPlayerNearby = false; // ตรวจสอบว่าผู้เล่นอยู่ใกล้ประตูหรือไม่

    void Start()
    {
        // ตั้งค่าตำแหน่งผู้เล่นเป็นตำแหน่งที่กำหนดไว้ใน customReturnPosition เสมอ
        player.transform.position = customReturnPosition;
        Debug.Log("ตั้งตำแหน่งเริ่มต้นของผู้เล่นเป็น customReturnPosition: " + customReturnPosition);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(interactKey))
        {
            // ไม่จำเป็นต้องบันทึกตำแหน่งผู้เล่นอีกต่อไป ให้ตั้งค่าใน customReturnPosition
            SceneManager.LoadScene(sceneToLoad);  // โหลด Scene ใหม่ตามที่ระบุไว้ใน sceneToLoad
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
