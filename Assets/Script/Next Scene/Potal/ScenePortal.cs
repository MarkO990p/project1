using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    public string destinationScene;
    public string entranceId;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // บันทึก EntranceID และข้อมูลเกมก่อนเปลี่ยนฉาก
            PlayerPrefs.SetString("EntranceID", entranceId);

            // บันทึกข้อมูลเกม (HP, ตำแหน่ง, ฯลฯ) ผ่าน DataPersistenceManager
            if (DataPersistenceManager.instance != null)
            {
                DataPersistenceManager.instance.SaveGame();
            }
            else
            {
                Debug.LogWarning("DataPersistenceManager instance not found!");
            }

            // โหลดฉากใหม่
            SceneManager.LoadScene(destinationScene);
        }
    }
}