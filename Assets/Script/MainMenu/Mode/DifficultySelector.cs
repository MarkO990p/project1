using UnityEngine;
using UnityEngine.SceneManagement; // ถ้าจะโหลดซีนต่อหลังเลือก
using UnityEngine.UI;

public enum DifficultyLevel { Easy, Medium, Hard }

public static class GameSettings
{
    public static DifficultyLevel CurrentDifficulty = DifficultyLevel.Medium;
}

public class DifficultySelector : MonoBehaviour
{
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    private void Start()
    {
        easyButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Easy));
        mediumButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Medium));
        hardButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Hard));
    }

    void SelectDifficulty(DifficultyLevel level)
    {
        GameSettings.CurrentDifficulty = level;
        Debug.Log("Selected difficulty: " + level);

        // ถ้าต้องการไปต่อในเกมหลังเลือก
        // SceneManager.LoadScene("GameScene");
    }
}
