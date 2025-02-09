using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;       // อ้างอิงไปยังแผง PauseMenu
    public Button resumeButton;        // ปุ่ม Resume
    public Button mainMenuButton;      // ปุ่ม Main Menu
    public Slider volumeSlider;        // Slider สำหรับปรับระดับเสียง
    public AudioSource backgroundAudio; // อ้างอิงไปยัง AudioSource ที่เล่นเพลงพื้นหลัง

    private bool isPaused = false;     // สถานะเกมว่าถูกหยุดหรือไม่

    void Start()
    {
        // ซ่อนแผง PauseMenu ตอนเริ่มต้น
        pauseMenu.SetActive(false);

        // ตั้งค่าการทำงานของปุ่ม
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);

        // ตั้งค่า Slider เริ่มต้นจากระดับเสียงปัจจุบันของ AudioSource
        if (backgroundAudio != null && volumeSlider != null)
        {
            volumeSlider.value = backgroundAudio.volume;
            volumeSlider.onValueChanged.AddListener(AdjustVolume);
        }
    }

    void Update()
    {
        // ตรวจสอบการกดปุ่ม Esc เพื่อสลับสถานะหยุด/เริ่มเกม
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // ฟังก์ชันสำหรับหยุดเกม
    public void PauseGame()
    {
        pauseMenu.SetActive(true); // แสดงแผง PauseMenu
        Time.timeScale = 0f;       // หยุดเวลาในเกม
        isPaused = true;

        // ลดระดับเสียงเมื่อเข้าสู่โหมด Pause
        if (backgroundAudio != null)
        {
            backgroundAudio.volume *= 0.5f; // ลดเสียงลง 50%
        }
    }

    // ฟังก์ชันสำหรับเล่นเกมต่อ
    public void ResumeGame()
    {
        pauseMenu.SetActive(false); // ซ่อนแผง PauseMenu
        Time.timeScale = 1f;        // ให้เวลาในเกมทำงานปกติ
        isPaused = false;

        // ปรับระดับเสียงกลับเป็นปกติเมื่อกลับมาเล่นเกมต่อ
        if (backgroundAudio != null)
        {
            backgroundAudio.volume = volumeSlider.value; // คืนค่าเสียงจาก Slider
        }
    }

    // ฟังก์ชันสำหรับปรับระดับเสียงตาม Slider
    private void AdjustVolume(float volume)
    {
        if (backgroundAudio != null)
        {
            backgroundAudio.volume = volume;
        }
    }

    // ฟังก์ชันสำหรับไปยังเมนูหลัก
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // ให้เวลาในเกมทำงานปกติ
        SceneManager.LoadScene("MainMenu"); // โหลด Scene เมนูหลัก
    }
}
