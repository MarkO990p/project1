using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // แผง GameOver ที่จะปรากฏขึ้น
    public Button respawnButton;     // อ้างอิงไปยังปุ่ม Respawn
    public Button mainMenuButton;    // อ้างอิงไปยังปุ่ม Main Menu
    public RespawnManager respawnManager; // อ้างอิงถึง RespawnManager
    private Health playerHealth;     // อ้างอิงถึง Health ของผู้เล่น

    void Start()
    {
        if (respawnButton != null)
            respawnButton.onClick.AddListener(Respawn);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        gameOverPanel.SetActive(false); // ซ่อนแผง GameOver ตอนเริ่มต้น

        // หา Health ของผู้เล่น
        playerHealth = FindObjectOfType<Health>();
    }

    public void TriggerGameOver()
    {
        gameOverPanel.SetActive(true); // แสดงแผง GameOver
        Time.timeScale = 0f; // หยุดเวลาในเกม
    }

    // ฟังก์ชันสำหรับเกิดใหม่
    void Respawn()
    {
        gameOverPanel.SetActive(false); // ซ่อนแผง GameOver
        Time.timeScale = 1f; // เริ่มเกมใหม่

        if (respawnManager != null)
        {
            respawnManager.RespawnPlayer(); // เรียกใช้ RespawnPlayer จาก RespawnManager
        }
        else
        {
            Debug.LogError("RespawnManager is not assigned in GameOverManager!");
        }
    }

    // ฟังก์ชันสำหรับไปยังเมนูหลัก
    void GoToMainMenu()
    {
        Time.timeScale = 1f; // ให้เกมเริ่มเดินต่อ
        SceneManager.LoadScene("MainMenu"); // โหลด Scene ของเมนูหลัก
    }
}
