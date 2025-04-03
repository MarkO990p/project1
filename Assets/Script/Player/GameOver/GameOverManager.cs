using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public Button respawnButton;
    public Button mainMenuButton;
    public RespawnManager respawnManager;

    private Health playerHealth;

    void Start()
    {
        // หา RespawnManager อัตโนมัติถ้าไม่เซ็ตจาก Inspector
        if (respawnManager == null)
        {
            respawnManager = FindObjectOfType<RespawnManager>();
            if (respawnManager == null)
            {
                Debug.LogWarning("RespawnManager not found in scene!");
            }
        }

        // ผูกฟังก์ชันให้ปุ่มทำงาน
        if (respawnButton != null)
            respawnButton.onClick.AddListener(Respawn);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        // ปิด Panel ตอนเริ่มเกม
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }



    public void TriggerGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Respawn()
    {
        Debug.Log("Trying to Respawn...");

        if (respawnManager == null)
        {
            respawnManager = FindObjectOfType<RespawnManager>();
            if (respawnManager == null)
            {
                Debug.LogError("RespawnManager NOT FOUND!");
                return;
            }
        }

        Debug.Log("RespawnManager FOUND. Respawning...");
        respawnManager.RespawnPlayer();
        gameOverPanel.SetActive(false);
    }


    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
